using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityLibp2p
{
    public class Libp2pConfig
    {
        // A Libp2p peerId is
        // pid = new { id = "string Id", pubKey = "string PublicKey" }

        // String definitions
        // Here in C#-land we are using text labels to specify what in javascript-land are *instances*
        // of modules or interfaces or whatever.
        public const string Websockets = "Websockets-inst", WebRTCStar = "WebRTCStar-inst"; // Transport modules
        public const string NOISE = "NOISE-inst"; // encryption
        public const string Mplex = "Mplex-inst"; // stream multiplexer
        public const string Bootstrap = "Bootstrap-inst";  // PeerDiscovery  modules
        public const string GossipSub = "GossipSub-inst", Floodsub = "Floodsub-inst"; // pubsub modules

        // Config tags:
        // These ket used in the JS config files, and a looked up as static class vars (Bootstrap.tag)
        // But in practice are actually just contants. I'll put in a test in the JS to signal if they ever change,
        // but other than that am going to treat them as string constants here.
        public const string BootstrapTag = "bootstrap";

        // config types
        // Note that bools are represented as strings so 'null' is a possobel value meaning "don't serialize"
        public const string bTRUE = "true";
        public const string bFALSE = "false";

        public class Addresses {
            public List<string> listen;
        }

        public class Modules {
            public List<string> transport;
            public List<string> connEncryption;
            public List<string> streamMuxer;
            public List<string> peerDiscovery;
            public string pubsub;
        }

        public class BootstrapPDConfig {
            public string enabled;
            public List<string> list;

        }
        public class WebRTCStarPDConfig {
            public string enabled;
        }

        public class PeerDiscoveryConfig {
            public string autoDial;
            public BootstrapPDConfig bootstrap;
            public WebRTCStarPDConfig webRTCStar;
        }

         public class PubSubConfig {
            public string enabled;
            public string emitSelf;
        }

        public class AutoRelayConfig {
            public string enabled;
            public int maxListeners; //
        }

       public class HopConfig {
            public string enabled;
        }


        public class RelayConfig {
            public string enabled;
            public AutoRelayConfig autoRelay;
             public HopConfig hop;
        }

        public class Config {
            public PeerDiscoveryConfig peerDiscovery;
            public PubSubConfig pubsub;
            public RelayConfig relay;

        }


        // Properties
        public Libp2pPeerId peerId;
        public Addresses addresses;
        public Modules modules;
        public Config config;

        public string ToJson() =>  JsonConvert.SerializeObject(this,  Formatting.Indented,
            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

        public static Libp2pConfig FromJson(string json) => JsonConvert.DeserializeObject<Libp2pConfig>(json);


        // static exmaple/defaults
        public static Libp2pConfig DefaultWebsocketConfig = new Libp2pConfig() {
            peerId = null,
            addresses = new Addresses(),
            modules = new Modules() {
                transport = new List<string>() {Websockets},
                connEncryption = new List<string>() {NOISE},
                streamMuxer =  new List<string>() {Mplex},
                peerDiscovery = new List<string>() { Bootstrap },
                pubsub = GossipSub
            },
            config = new Config() {
                peerDiscovery = new PeerDiscoveryConfig() {
                    autoDial = bTRUE,
                    bootstrap = new BootstrapPDConfig() {
                        enabled = bTRUE,
                        list = new List<string>() { "bootstrapAddr1" } // Must be replaced before submitting
                    }
                },
                pubsub = new PubSubConfig() {
                    enabled = bTRUE,
                    emitSelf = bTRUE
                },
                relay = new RelayConfig {
                    enabled = bTRUE,
                    autoRelay = new AutoRelayConfig {
                        enabled = bTRUE,
                        maxListeners = 2
                    },
                    hop = new HopConfig {
                        enabled = bTRUE
                    }
                }
            }
        };


         public static object ExampleFullLiteralConfig => new {
            peerId = new {
                id = "THEID",
                pubKey = "THEPUBKEY",
                privKey = "THEPRIVKEY"
            } ,
            addresses = new {
                listen = new []{"listenAddr1", "listenAddr2" }
            },
            modules = new {
                transport = new [] {Websockets, WebRTCStar},
                connEncryption = new [] {NOISE},
                streamMuxer = new [] {Mplex},
                peerDiscovery = new [] { Bootstrap },
                pubsub = GossipSub
            },
            config = new {
                peerDiscovery = new Dictionary<string, object>(){  // have to explicitly use a dict because tag name is a variable. Even tho it's const...
                    { "autoDial",  true},
                    { BootstrapTag,  new {
                            enabled = true,
                            list = new [] { "bootstrapAddr1", "bootstrapAddr2" }
                        }
                    }
                },
                pubsub = new {
                    enabled = true,
                    emitSelf = false
                },
                relay = new {
                    enabled = true,
                    hop = new {
                        enabled = true
                    }
                }
            }
        };

        public static object ExampleWebsocketsConfig => new {
            // addresses = new {
            //     listen = new dynamic ["listenAddr1", "listenAddr2" ]  Websockets neesd to dial a relay(s) to get a listen address
            // },
            modules = new {
                transport = new [] { Websockets },
                connEncryption = new [] {NOISE},
                streamMuxer = new [] {Mplex},
                peerDiscovery = new [] { Bootstrap },
                pubsub = GossipSub
            },
            config = new {
                peerDiscovery = new Dictionary<string, object>(){  // have to explicitly use a dict because tag name is a variable. Even tho it's const...
                    { "autoDial",  true},
                    {  BootstrapTag,  new {
                            enabled = true,
                            list = new [] {
                                "/dns4/newsweasel.com/tcp/15003/wss/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8"
                            }
                        }
                    }
                },
                pubsub = new {
                    enabled = true,
                    emitSelf = false
                },
                relay = new {
                    enabled = true,
                    autoRelay = new {
                        enabled = true,
                        maxListeners = 2
                    },
                    hop = new {
                        enabled = true
                    }
                }
            }
        };

        public static object ExampleWebRTCStarConfig => new {
            addresses = new {
                listen = new [] { "WebRTCStar-sentinel-1", "WebRTCStar-sentinel-1" } // WebRTCStar protocol uses this to, well, dial, and then add listen addresses
            },
            modules = new {
                transport = new [] { Websockets },
                connEncryption = new [] {NOISE},
                streamMuxer = new [] {Mplex},
                // peerDiscovery = new dynamic [LibP2pCfg.Bootstrap],
                pubsub = GossipSub
            },
            config = new {
                // peerDiscovery = new Dictionary<string, object>(){  // We COULD add bootstrap peers if we know them.
                //     {
                //         LibP2pCfg.BootstrapTag = new {
                //             enabled = true,
                //             list = new dynamic ["relayAddr1", "relayAddr2"]
                //         }
                //     }
                // },
                pubsub = new {
                    enabled = true,
                    emitSelf = false
                },
                relay = new {
                    enabled = true,
                    hop = new {
                        enabled = true
                    }
                }
            }
        };

    }

}