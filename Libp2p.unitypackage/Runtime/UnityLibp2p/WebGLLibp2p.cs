using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
#endif

namespace UnityLibp2p
{

#if UNITY_WEBGL

    public class WebGLLibp2p : ILibp2p
    {
        //
        // Static stuff
        //

        public static Dictionary<string, WebGLLibp2p> LibInstances;

        // Emscripten-compiled Javascript functions
	    [DllImport("__Internal")]
	    private static extern void JsLibp2p_InitCallbacks( Action<string, string> onCreatedCb, Action<string> onStartedCb,
            Action<string, string> onDiscoveryCb, Action<string, string, bool> onConnectionCb,
            Action<string, string> onListenAddrCb, Action<string, string, string, string> onMessageCb,
            Action<string> onStoppedCb, Action<string, string, int> onPingCb );


        // These declarations are ONLY to keep Emscripten from stripping these funcs.
        // (They are the JS/C# interop funcs that get called from JS to report back to us here)
        [DllImport("__Internal")]
        private static extern void JsLibp2p_CreateFromNamedCfg(string instId, string cfgName, string cfgOpts);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_CreateFromConfig(string instId, string configJson);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_StartLib(string instId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_StopLib(string instId);

        [DllImport("__Internal")]
        private static extern bool JsLibp2p_Subscribe(string instId, string topic);

        [DllImport("__Internal")]
        private static extern bool JsLibp2p_Unsubscribe(string instId, string topic);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_Publish(string instId, string topic, string payload);

        [DllImport("__Internal")]
        private static extern bool JsLibp2p_Dial(string instId, string peerAddr);

        [DllImport("__Internal")]
        private static extern bool JsLibp2p_HangUp(string instId, string peerAddr);

        [DllImport("__Internal")]
        private static extern bool JsLibp2p_Ping(string instId, string peerAddr);

        // callbacks
        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnLibCreated( string clientId, string localPeerJson);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnLibStarted(string libId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnDiscovery(string libId, string peerId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnConnection(string libId, string peerId, bool connected);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnListenAddress(string libId, string addrArrayJson);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnMessage(string libId, string sourceId, string topic, string message);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnLibStopped(string libId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnPing(string libId, string peerAddr, int latencyMs);

        // Static ctor sets up JS callback pointers
        static WebGLLibp2p()
        {
            LibInstances = new Dictionary<string, WebGLLibp2p>();
            JsLibp2p_InitCallbacks( JsLibp2p_OnLibCreated_Cb, JsLibp2p_OnLibStarted_Cb, JsLibp2p_OnDiscovery_Cb, JsLibp2p_OnConnection_Cb,
            JsLibp2p_OnListenAddress_Cb, JsLibp2p_OnMessage_Cb, JsLibp2p_OnLibStopped_Cb, JsLibp2p_OnPing_Cb );
        }

        // Static callbacks from javascript
       [MonoPInvokeCallback(typeof(Action<string>))]
        public static void JsLibp2p_OnLibCreated_Cb(string libId, string localPeerJson)
        {
            Debug.Log($"JsLibp2p_OnCreated_Cb() - LibId: {libId}");
            try {
                LibInstances[libId].OnCreated(localPeerJson);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

       [MonoPInvokeCallback(typeof(Action<string>))]
        public static void JsLibp2p_OnLibStarted_Cb(string libId)
        {
            Debug.Log($"JsLibp2p_OnLibStarted_Cb() - LibId: {libId}");
            try {
                LibInstances[libId].OnStarted();
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        public static void JsLibp2p_OnDiscovery_Cb(string libId, string peerJson)
        {
            Debug.Log($" JsLibp2p_OnDiscovery_Cb()! LibId: {libId} PeerId: {peerJson}");
            try {
                LibInstances[libId].OnPeerDiscovery(peerJson);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string, bool>))]
        public static void JsLibp2p_OnConnection_Cb(string libId, string peerJson, bool connected)
        {
            Debug.Log($"JsLibp2p_OnConnection_Cb() LibId: {libId} PeerId: {peerJson} Connected: {connected}");
            try {
                LibInstances[libId].OnConnectionEvent(peerJson, connected);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        public static void JsLibp2p_OnListenAddress_Cb(string libId, string addrArrayJson)
        {
            Debug.Log($" JsLibp2p_ListenAddress_Cb()! LibId: {libId} ListenAddrs: {addrArrayJson}");
            try {
                LibInstances[libId].OnListenAddress(addrArrayJson);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string, string, string>))]
        public static void JsLibp2p_OnMessage_Cb(string libId, string sourceId, string topic, string msgStr)
        {
            Debug.Log($"JsLibp2p_OnMessage_Cb() LibId: {libId} SourceId: {sourceId}, Topic: {topic}");
            try {
                LibInstances[libId].OnMessage(sourceId, topic, msgStr);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

       [MonoPInvokeCallback(typeof(Action<string>))]
        public static void JsLibp2p_OnLibStopped_Cb(string libId)
        {
            Debug.Log($"JsLibp2p_OnLibStopped_Cb() - LibId: {libId}");
            try {
                LibInstances[libId].OnStopped();
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string, int>))]
        public static void JsLibp2p_OnPing_Cb(string libId, string peerAddr, int latencyMs)
        {
            try {
                LibInstances[libId].OnPing(peerAddr, latencyMs);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        //
        // Factory
        //
        public static ILibp2p Factory(ILibp2pClient client, Libp2pConfig config)
        {
            string instanceId = Guid.NewGuid().ToString();

            WebGLLibp2p inst = new WebGLLibp2p(instanceId, client);
            LibInstances[instanceId] = inst;

            Debug.Log($"WebGLLibp2p.Factory() Creating js-libp2p instance for {instanceId}");

            JsLibp2p_CreateFromConfig(instanceId,  config.ToJson());

            return inst;
        }


        // public static ILibp2p CreateNamedConfig(string configName, string configOptions)
        // {
        //     // TODO: Can this be synchronous? Options:
        //     //  a) It's  synchronous and the interop func returns quickly and it just goes
        //     //  b) It needs to wait:
        //     //      1) The factory is synchronous - creates and returns an unitialized proxy object and everyone waits for a "ready" callback
        //     //      2) factory is async and pretty much does 1) - but doesn;t return until the callback fires
        //     //      3) factory is async and there's some clever way to wait here for the interop code to return async (DON'T THINK SO)
        //     string instanceId = Guid.NewGuid().ToString();

        //     WebGLLibp2p inst = new WebGLLibp2p(instanceId);
        //     LibInstances[instanceId] = inst;

        //     // Just to test this...
        //     // JsLibp2p_CreateFromConfig(instanceId,  JsonConvert.SerializeObject(Libp2pConfig.ExampleFullLiteralConfig));


        //     JsLibp2p_CreateFromNamedCfg(instanceId, configName, configOptions);
        //     return inst;
        // }

        //
        // Instance stuff
        //

        public string InstanceId { get; private set; }
        public ILibp2pClient Client { get; private set; }

        public bool IsStarted { get; private set; }
        public Libp2pPeerId PeerId { get; private set;}
        public List<string> ListenAddrs {get; private set;}

        protected WebGLLibp2p(string instanceId, ILibp2pClient client)
        {
            InstanceId = instanceId;
            Client = client;
            IsStarted = false;
        }

        // ILibP2p
        public void Start()
        {
            JsLibp2p_StartLib(InstanceId);
        }

        public void Stop()
        {
            JsLibp2p_StopLib(InstanceId); // does NOT delete anything. Just calls stop()
        }

        public bool Subscribe(string topic)
        {
            return JsLibp2p_Subscribe(InstanceId, topic);
        }

        public bool  Unsubscribe(string topic)
        {
            return JsLibp2p_Unsubscribe(InstanceId, topic);
        }

        public void Publish(string topic, string payload)
        {
            JsLibp2p_Publish(InstanceId, topic, payload);
        }

        public void Dial( string peerAddr)
        {
            JsLibp2p_Dial(InstanceId, peerAddr);
        }

        public void HangUp( string peerAddr)
        {
            JsLibp2p_HangUp(InstanceId, peerAddr);
        }

        public void Ping( string peerAddr)
        {
            JsLibp2p_Ping(InstanceId, peerAddr);
        }

        // Client Callback "callers"
        protected void OnCreated(string localPeerJson)
        {
            Debug.Log($"WebGLLibp2p.OnCreated(): libId: {InstanceId}, localPeer: {localPeerJson}");

            var peerDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(localPeerJson);
            Libp2pPeerId localPeer = Libp2pPeerId.FromDict(peerDict);

            PeerId = localPeer;
            Client.OnCreated(localPeer);
        }

        protected void OnStarted()
        {
            IsStarted = true;
            Debug.Log($"WebGLLibp2p.OnStarted(): libId: {InstanceId}");
            Client.OnStarted();
        }

        protected void OnStopped()
        {
            IsStarted = false;
            Debug.Log($"WebGLLibp2p.OnStopped(): libId: {InstanceId}");
            Client.OnStopped();
        }

        protected void OnPeerDiscovery(string peerJson)
        {
            // TODO: also take multiaddress as an arg
            var peerDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(peerJson);
            Libp2pPeerId peerId = Libp2pPeerId.FromDict(peerDict);
            Debug.Log($"WebGLLibp2p.OnPeerDiscovery(): {peerId}");
            Client.OnPeerDiscovery(peerId);
            // TODO: update list of known peers
        }

        protected void OnConnectionEvent(string peerJson, bool connected)
        {
            // TODO: update known peers 'connected' property
            var peerDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(peerJson);
            Libp2pPeerId peerId = Libp2pPeerId.FromDict(peerDict);
            Debug.Log($"WebGLLibp2p.OnConnected(): {peerId} {(connected ? "Connected" : "Disconnected")}");
            Client.OnConnectionEvent(peerId, connected);
        }

        protected void OnListenAddress(string addrArrayJson)
        {
            // TODO: update local peer "listenAddresses" property
            var addrList = JsonConvert.DeserializeObject<List<string>>(addrArrayJson);
            ListenAddrs = addrList;
            Debug.Log($"WebGLLibp2p.OnListenAddress(): libId: {InstanceId}, listenAddresses: {addrArrayJson}");
            Client.OnListenAddress( addrList );
        }

        protected void OnMessage(string sourceId, string topic, string msgStr)
        {
            Debug.Log($"WebGLLibp2p.OnMessage()");
            Client.OnMessage( sourceId, topic, msgStr);
        }

        protected void OnPing(string peerAddr, int latencyMs)
        {
            Debug.Log($"WebGLLibp2p.OnPing()");
            Client.OnPing( peerAddr, latencyMs );
        }

    }

#endif // UNITY_WEBGL

}