using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
#endif

namespace UnityLibp2p
{

#if UNITY_WEBGL

    public class WebGLLibp2p : Libp2p
    {
        //
        // Static stuff
        //

        public static Dictionary<string, WebGLLibp2p> LibInstances;

        // Emscripten-compiled Javascript functions
	    [DllImport("__Internal")]
	    private static extern void JsLibp2p_InitCallbacks( Action<string> onCreatedCb, Action<string> onStartedCb,
            Action<string, string> onDiscoveryCb, Action<string, string, bool> onConnectionCb,
            Action<string, string, string> onMessageCb  );


        // These declarations are ONLY to keep Emscripten from stripping these funcs.
        // (They are the JS/C# interop funcs that get called from JS to report back to us here)

        [DllImport("__Internal")]
        private static extern void JsLibp2p_CreateFromNamedCfg(string instId, string cfgName, string cfgOpts);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_StartLib(string instId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnLibCreated( string clientId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnLibStarted(string libId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnDiscovery(string libId, string peerId);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnConnection(string libId, string peerId, bool connected);

        [DllImport("__Internal")]
        private static extern void JsLibp2p_OnMessage(string libId, string sourceId, string message);



        static WebGLLibp2p()
        {
            LibInstances = new Dictionary<string, WebGLLibp2p>();
            JsLibp2p_InitCallbacks( JsLibp2p_OnLibCreated_Cb, JsLibp2p_OnLibStarted_Cb,
                JsLibp2p_OnDiscovery_Cb,  JsLibp2p_OnConnection_Cb, JsLibp2p_OnMessage_Cb );
        }

        // Static callbacks from javascript
       [MonoPInvokeCallback(typeof(Action<string>))]
        public static void JsLibp2p_OnLibCreated_Cb(string libId)
        {
            Debug.Log($"JsLibp2p_OnCreated_Cb() - LibId: {libId}");
            try {
                LibInstances[libId].OnCreated();
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
        public static void JsLibp2p_OnDiscovery_Cb(string libId, string peerId)
        {
            Debug.Log($" JsLibp2p_OnDiscovery_Cb()! LibId: {libId} PeerId: {peerId}");
            try {
                LibInstances[libId].OnPeerDiscovery(peerId);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string, bool>))]
        public static void JsLibp2p_OnConnection_Cb(string libId, string peerId, bool connected)
        {
            Debug.Log($"JsLibp2p_OnConnection_Cb() LibId: {libId} PeerId: {peerId} Connected: {connected}");
            try {
                LibInstances[libId].OnConnectionEvent(peerId, connected);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        [MonoPInvokeCallback(typeof(Action<string, string, string>))]
        public static void JsLibp2p_OnMessage_Cb(string libId, string sourceId, string msgStr)
        {
            Debug.Log($"JsLibp2p_OnMessage_Cb() LibId: {libId} SourceId: {sourceId}");
            try {
                LibInstances[libId].OnMessage(sourceId, msgStr);
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }

        public static ILibp2p CreateNamedConfig(string configName, string configOptions)
        {
            // TODO: Can this be synchronous? Options:
            //  a) It's  synchronous and the interop func returns quickly and it just goes
            //  b) It needs to wait:
            //      1) The factory is synchronous - creates and returns an unitialized proxy object and everyone waits for a "ready" callback
            //      2) factory is async and pretty much does 1) - but doesn;t return until the callback fires
            //      3) factory is async and there's some clever way to wait here for the interop code to return async (DON'T THINK SO)
            string instanceId = Guid.NewGuid().ToString();

            WebGLLibp2p inst = new WebGLLibp2p(instanceId);
            LibInstances[instanceId] = inst;
            JsLibp2p_CreateFromNamedCfg(instanceId, configName, configOptions);
            return inst;
        }

        //
        // Instance stuff
        //

        public string InstanceId { get; private set; }
        public bool IsStarted { get; private set; }

        protected WebGLLibp2p(string instanceId)
        {
            InstanceId = instanceId;
        }


        public void Start()
        {
            JsLibp2p_StartLib(InstanceId);
        }

        protected void OnCreated()
        {
            Debug.Log($"WebGLLibp2p.OnCreated(): libId: {InstanceId}");

            Start(); // TODO: this is just a test shortcut
        }

        protected void OnStarted()
        {
            IsStarted = true;
            Debug.Log($"WebGLLibp2p.OnStarted(): libId: {InstanceId}");
        }

        protected void OnPeerDiscovery(string peerId)
        {
            Debug.Log($"WebGLLibp2p.OnPeerDiscovery(): {peerId}");
        }

        protected void OnConnectionEvent(string peerId, bool connected)
        {
            Debug.Log($"WebGLLibp2p.OnConnected(): {peerId} {(connected ? "Connected" : "Disconnected")}");
        }


        protected void OnMessage(string sourceId, string msgStr)
        {
            Debug.Log($"WebGLLibp2p.OnMessage()");
        }

    }

#endif // UNITY_WEBGL

}