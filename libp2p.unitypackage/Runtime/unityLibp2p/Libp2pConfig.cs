using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibp2p
{

    public class ModuleOpts
    {
        // transport [WS, WebRTCStar]
        public const string Websockets = "Websockets", WebRTCStar = "WebRTCStar";
        public IEnumerable<string> transport;
        // streamMuxer - use [MPLEX]
        // connEncryption - use [NOISE]
        // peerDiscovery [Bootstrap, PubsubPeerDiscovery]
        // NOTE: the WebRTCStar tranport includes its own discovery
        // Also "Any DHT will offer you a discovery capability" with the right options
        // ( https://github.com/libp2p/js-libp2p/tree/master/examples/discovery-mechanisms#4-where-to-find-other-peer-discovery-mechanisms)
        public const string Bootstrap = "Bootstrap", PubsubPeerDiscovery = "PubsubPeerDiscovery";
        public IEnumerable<string> peerDiscovery;
        // dht - for now use default (I don;t even know what that is)
        // pubsub: GossipSub, Floodsub
        public const string GossipSub = "GossipSub", Floodsub = "Floodsub";
        public string pubsub;
    }

    public class BootstrapCfg // in config.peerDiscovery
    {
        bool emabled;
        IEnumerable<string> list; // Peer addrs: '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN'
    }

    public class ConfigOpts
    {
        public IEnumerable<object> peerDiscovery;
    }

    public class Libp2pConfig
    {
        // addresses
        // modules
        public ModuleOpts modules;
        // config
    }

}