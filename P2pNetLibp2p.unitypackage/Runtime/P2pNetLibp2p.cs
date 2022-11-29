using System;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniLog;
using UnityEngine;
using Libp2pUnity;
using P2pNet;


    public class P2pNetLibp2p : IP2pNetCarrier, ILibp2pClient
    {
       class JoinState
        {
            public SynchronizationContext mainSyncCtx; // might be null
            public IP2pNetBase p2pBase;
            public P2pNetChannelInfo mainChannel;
            public string localHelloData;
            public string ListenAddress;

            public Libp2pPeerId localLibp2pId;
            public bool IsConnected;
            public bool HasPeers; // are we connected to anyone subscribed to the main channel?
        };

        private JoinState joinState;

        protected ILibp2p lib;

        private readonly Dictionary<string,string> connectOpts;




        public UniLogger logger;

        public P2pNetLibp2p( string _connectionString)
        {
            logger = UniLogger.GetLogger("P2pNet");

            // {  "relaybase":"<relayBaseMaddr>"
            //    "relayid":<relay connect peerId>
            //    "dialid":<pubsub peer id>  <== ignore this
            // }
            connectOpts = JsonConvert.DeserializeObject<Dictionary<string,string>>(_connectionString);

            ResetJoinVars();
        }

        private void ResetJoinVars()
        {
            joinState = null;
        }

        public void Join(P2pNetChannelInfo mainChannel, IP2pNetBase p2pBase, string localHelloData)
        {
            ResetJoinVars();

            Libp2pConfig configObj = Libp2pConfig.DefaultWebsocketConfig ; // by default

            configObj.config.peerDiscovery.bootstrap.enabled = true;
            configObj.config.peerDiscovery.bootstrap.list[0] = connectOpts["relaybase"]+connectOpts["relayid"];

            joinState = new JoinState()
            {
                p2pBase=p2pBase,
                mainChannel=mainChannel,
                localHelloData=localHelloData,
                mainSyncCtx = SynchronizationContext.Current,
                ListenAddress = null,
                localLibp2pId = null,
                HasPeers = false,
                IsConnected = false
            };

            lib = Libp2p.Factory(this, configObj);

            // doesn't start until OnCreated...
        }


        public void Leave()
        {
            // FIXME: Do this

            // ConnectionMux.Close();
            // ConnectionMux =null;
            // customConnectFactory = null;
            // connectionString = null;
            // messageQueue = null;
        }

        public void Send(P2pNetMessage msg)
        {
            string msgJSON = JsonConvert.SerializeObject(msg);
            lib.Publish(msg.dstChannel, msgJSON);
        }

        public void Listen(string channel)
        {
            lib.Subscribe(channel);
        }

        public void StopListening(string channel)
        {
            lib.Unsubscribe(channel);
        }

        public void AddReceiptTimestamp(P2pNetMessage msg)
        {
            msg.rcptTime = P2pNetDateTime.NowMs;
        }

        public void Poll() {}

        // ILibp2pClient implementation
        public void OnCreated(Libp2pPeerId localPeer)
        {
            if (!lib.IsStarted)
            {
                joinState.localLibp2pId = localPeer;
                lib.Start();

                // results (async) in OnStarted
                // Also - because the relay in in the bootstrap list, there will be
                // a an OnConnected, too, with the relayer ID)
            }
        }
        public void OnStarted()
        {

            // wait until actually connectd (listen address AND started)
        }

        public void OnStopped()
        {
            //Log( $"\nLibp2p instance {lib.InstanceId} stopped.");
        }

        protected void _reportConnectedToNet()
        {
            Listen(joinState.p2pBase.LocalId);

            joinState.IsConnected = true;

            // OnNetworkJoined needs to be synchronized
            if (joinState.mainSyncCtx != null)
            {
                joinState.mainSyncCtx.Post( new SendOrPostCallback( (o) => {
                    joinState.p2pBase.OnNetworkJoined(joinState.mainChannel, joinState.localHelloData);
                } ), null);
            } else {
                joinState.p2pBase.OnNetworkJoined(joinState.mainChannel, joinState.localHelloData);
            }
        }

        public void OnListenAddress(List<string> addresses)
        {
            // We get this when we have connected to the relay and have been assigned a proxy
            //  "listen" address. At this point we can consider ourtselves "connected" to the
            // group's network, tho we probaly don;t ahve any connections yet.
            // Since this is NOT a brokered pubsub, if there are no peers then sending messages doesn;t do
            // anything at all.
            if (addresses.Count > 0)
            {
                if ( joinState.IsConnected == false)
                {
                    joinState.ListenAddress = addresses[0];
                    _reportConnectedToNet(); // this results in subscribing to localPeerId and mainchannel
                                             // It also sends an "I'm here" but it probably doesnt go anywhere
                                             // because we probably aren't conected to anyone yet.
                                             // Also sets IsCOnnected
                }
            }
        }
        public void OnPeerDiscovery(Libp2pPeerId peerId)
        {
            //Log( $"\nRemote peer discovered: {peerId.id}");
        }
        public void OnConnectionEvent(Libp2pPeerId peerId, bool connected)
        {
            // A peer has either connected or disconnected
            logger.Verbose( $"\n{(connected ? "Connected to" : "Disconnected from")}  remote peer: {peerId.id}");
            if ( connected == true) // the remote peer has connected
            {
                // SO NOW there's a peer connected. If we haven;t connected to anyone else (other than the relay) we should
                // send a broadcast to tell everyone  we're here
                if (joinState.IsConnected && peerId.id != connectOpts["relayid"]) // Note that the FIRST connection is the relay, but that's before we have a listen address
                {
                    string chId = joinState.mainChannel.id;
                    joinState.p2pBase.SendHelloMsg(chId, chId); // resend
                    joinState.HasPeers = true; //  Too optimistic?
                }
            }

        }
        public void OnMessage(string sourceId, string topic, string payload)
        {
            logger.Verbose($"P2pJsLibp2p OnMessage() thread: {Environment.CurrentManagedThreadId}");
            P2pNetMessage msg = JsonConvert.DeserializeObject<P2pNetMessage>(payload);

            AddReceiptTimestamp(msg);

            if (joinState.mainSyncCtx != null)
            {
                joinState.mainSyncCtx.Post( new SendOrPostCallback( (o) => {
                    logger.Verbose($"XXXXXXXX thread: {Environment.CurrentManagedThreadId}");
                    joinState.p2pBase.OnReceivedNetMessage(msg.dstChannel, msg);
                } ), null);
            } else {
                joinState.p2pBase.OnReceivedNetMessage(msg.dstChannel, msg);
            }
        }
        public void OnPing(string peerAddr, int latencyMs)
        {
            //Log( $"Ping returned from {peerAddr} - latency: {latencyMs} ms");
        }

    }

