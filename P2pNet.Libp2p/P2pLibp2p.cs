using System;
using System.Collections.Generic;
using UnityLibp2p;
using Newtonsoft.Json;

using UnityEngine;

namespace P2pNet
{

    public class P2pLibp2p : P2pNetBase, ILibp2pClient
    {
        private readonly object queueLock = new object();
        private Queue<P2pNetMessage> rcvMessageQueue;

        private readonly Dictionary<string,string> connectOpts;
        public ILibp2p lib;
        public Libp2pPeerId localLibp2pId;

        public string ListenAddress { get; private set; }

        public bool IsConnected { get; private set;}

        protected P2pNetChannelInfo mainChannelInfo;
        protected string mainHelloData;

        public P2pLibp2p(IP2pNetClient _client, string _connectionString) : base(_client, _connectionString)
        {
            rcvMessageQueue = new Queue<P2pNetMessage>();

            // {  "relaybase":"<relatBaseMaddr>"
            //    "relayid":<relay connect peerId>
            //    "dialid":<pubsub peer id>
            // }
            connectOpts = JsonConvert.DeserializeObject<Dictionary<string,string>>(_connectionString);

            GetId();
        }


        protected override void CarrierProtocolPoll()
        {
            // receive polling
            if (rcvMessageQueue.Count > 0)
            {
                Queue<P2pNetMessage> prevMessageQueue;
                lock(queueLock)
                {
                    prevMessageQueue = rcvMessageQueue;
                    rcvMessageQueue = new Queue<P2pNetMessage>();
                }

                foreach( P2pNetMessage msg in prevMessageQueue)
                {
                    OnReceivedNetMessage(msg.dstChannel, msg);
                }
            }
        }

        protected override void CarrierProtocolJoin(P2pNetChannelInfo mainChannel, string localPeerId, string localHelloData)
        {

            Libp2pConfig configObj = Libp2pConfig.DefaultWebsocketConfig ; // by default

            configObj.config.peerDiscovery.bootstrap.list[0] = connectOpts["relaybase"]+connectOpts["relayid"];

            mainChannelInfo = mainChannel;
            mainHelloData = localHelloData;

            lib = Libp2p.Factory(this, configObj);


            // doesn't start until OnCreated...

            // CarrierProtocolListen(localPeerId);
            // OnNetworkJoined(mainChannel, localHelloData);
        }

        protected override void CarrierProtocolLeave()
        {
            // ConnectionMux.Close();
            // ConnectionMux =null;
            // customConnectFactory = null;
            // connectionString = null;
            // messageQueue = null;
        }

        protected override void CarrierProtocolSend(P2pNetMessage msg)
        {
            string msgJSON = JsonConvert.SerializeObject(msg);
            lib.Publish(msg.dstChannel, msgJSON);

            // ConnectionMux.GetSubscriber().PublishAsync(msg.dstChannel, msgJSON);
        }

        protected override void CarrierProtocolListen(string channel)
        {
            lib.Subscribe(channel);
        }


        // private void _ListenSequential(string channel)
        // {
        //     var rcvChannel = ConnectionMux.GetSubscriber().Subscribe(channel);

        //     rcvChannel.OnMessage(channelMsg =>
        //     {
        //         P2pNetMessage msg = JsonConvert.DeserializeObject<P2pNetMessage>(channelMsg.Message);
        //         CarrierProtocolAddReceiptTimestamp(msg);
        //         lock(queueLock)
        //             messageQueue.Add(msg); // queue it up
        //     });
        // }

        protected override void CarrierProtocolStopListening(string channel)
        {
            //ConnectionMux.GetSubscriber().Unsubscribe(channel);
        }

        protected override string CarrierProtocolNewP2pId()
        {
            return System.Guid.NewGuid().ToString();
        }

        protected override void CarrierProtocolAddReceiptTimestamp(P2pNetMessage msg)
        {
            msg.rcptTime = P2pNetDateTime.NowMs;
        }

    // ILibp2pClient implementation
        public void OnCreated(Libp2pPeerId localPeer)
        {
            if (!lib.IsStarted)
            {
                localLibp2pId = localPeer;
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
            IsConnected = true;
            CarrierProtocolListen(GetId()); // pass **P2pNet** ID as channel name (lot libp2p id)
            OnNetworkJoined(mainChannelInfo, mainHelloData);

        }

        public void OnListenAddress(List<string> addresses)
        {
            // We get this when we have connected to the realy and have been assigned a proxy
            //  "listen" address. At this point if we created the group and theso there are no other
            // members ( so connectOpts["dialid"] is the empty string), then we can consider
            // ourtselves "connected" to the group's network
            if (addresses.Count > 0)
            {
                if (!IsConnected)
                {
                    ListenAddress = addresses[0];

                    if (connectOpts["dialid"] == "")
                    {
                        _reportConnectedToNet();
                    } else{
                        // TODO: this all assumes dialId's maddr is relaybase+dialid.
                        // This might not be true
                        lib.Dial(connectOpts["relaybase"]+connectOpts["dialid"]); // wait until we are connected to another peer
                    }
                }
            }
        }
        public void OnPeerDiscovery(Libp2pPeerId peerId)
        {
            //Log( $"\nRemote peer discovered: {peerId.id}");
        }
        public void OnConnectionEvent(Libp2pPeerId peerId, bool connected)
        {
            //Log( $"\n{(connected ? "Connected to" : "Disconnected from")}  remote peer: {peerId.id}");
            if ( connected == true)
            {
                if (IsConnected == false)
                {
                    if (peerId.id == connectOpts["dialid"])
                    {
                        // we are now connected to a pubsub peer so can start talking
                        _reportConnectedToNet();
                    }
                }
            }

        }
        public void OnMessage(string sourceId, string topic, string payload)
        {
            P2pNetMessage msg = JsonConvert.DeserializeObject<P2pNetMessage>(payload);
             CarrierProtocolAddReceiptTimestamp(msg);
            lock(queueLock)
                rcvMessageQueue.Enqueue(msg); // queue it up
        }
        public void OnPing(string peerAddr, int latencyMs)
        {
            //Log( $"Ping returned from {peerAddr} - latency: {latencyMs} ms");
        }

    }
}
