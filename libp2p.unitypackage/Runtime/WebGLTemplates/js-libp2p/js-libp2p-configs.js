const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;


var TRANSPORT_WEBSOCKETS = 'WEBSOCKETS'
var TRANSPORT_WEBRTCSTAR = 'WEBRTCSTAR'

var PUBSUB_GOSSIP = 'GOSSIPSUB'
var PUBSUB_FLOOD = 'FLOODSUB'

export default {

  BuildPubsubConfig: function(peerId, pubsubType, transports, listenAddrs, bootstrapAddrs, emitSelf )
  {
    var config =  {
      modules: {
        transport: [],
        connEncryption: [NOISE],
        streamMuxer: [Mplex],
        pubsub: null,
      },
      config: {
        pubsub: {
          enabled: true,
          emitSelf: false
        },
        relay: {
          enabled: true,
          hop: {
            enabled: true
          }
        },
      }
    }

    console.log(`BuildPubsubConfig(): Creating config`)

    // PeerID
    if (peerId) {
      config.peerId = peerId
      console.log(`BuildPubsubConfig(): Specifying peer: ${JSON.stringify(peerId)}`)
    }

    // Pubsub type
    if ( pubsubType === PUBSUB_GOSSIP )
      config.modules.pubsub = GossipSub
    else if (pubsubType === PUBSUB_FLOOD )
      config.modules.pubsub = FloodSub
    else {
      console.error(`BuildPubsubConfig(): Invalid pubsub type: \"${pubsubType}\"`)
      return null;
    }

    // Transports
    if (transports && transports.includes(TRANSPORT_WEBSOCKETS))
      config.modules.transport.push(Websockets)

    if (transports && transports.includes(TRANSPORT_WEBRTCSTAR))
      config.modules.transport.push(WebRTCStar)

    if (config.modules.transport.length === 0) {
      console.error(`BuildPubsubConfig(): No valid transports in \"${JSON.stringify(transports)}\"`)
      return null;
    }
    console.log(`BuildPubsubConfig(): Added transports: ${JSON.stringify(transports)}`)

    // Listen addresses
    if (listenAddrs && listenAddrs.length > 0) {
      console.log(`BuildPubsubConfig(): Adding listenAddrs: ${JSON.stringify(listenAddrs)}`)
      config.addresses =
      {
        listen: listenAddrs
      }
    }

    // Bootstrap?
    if (bootstrapAddrs && bootstrapAddrs.length > 0) {
      console.log(`BuildPubsubConfig(): Adding bootstrapAddrs ${JSON.stringify(bootstrapAddrs)}`)

      config.modules.peerDiscovery = [Bootstrap]

      config.config.peerDiscovery =
        {
          [Bootstrap.tag]: {
            enabled: true,
            list: bootstrapAddrs
          }
      }
    }

    // pubsub.emitSelf?
    console.log(`BuildPubsubConfig(): Pubsub emitself set to ${emitSelf}`)
    config.config.pubsub.emitSelf = emitSelf

    return config
  },

  Libp2p_configs: {

    WebRtcStar_Bs_Gossip_config: {
      addresses: {
        // Add the signaling server address, along with our PeerId to our multiaddrs list
        // libp2p will automatically attempt to dial to the signaling server so that it can
        // receive inbound connections from other peers
        listen: [
          '/dns4/wrtc-star1.par.dwebops.pub/tcp/443/wss/p2p-webrtc-star',
          '/dns4/wrtc-star2.sjc.dwebops.pub/tcp/443/wss/p2p-webrtc-star'
        ]
      },
      modules: {
        transport: [Websockets, WebRTCStar],
        connEncryption: [NOISE],
        streamMuxer: [Mplex],
        peerDiscovery: [Bootstrap],
        pubsub: GossipSub
      },
      config: {
        pubsub: {
          enabled: true,
          emitSelf: false
        },
        peerDiscovery: {
          // The `tag` property will be searched when creating the instance of your Peer Discovery service.
          // The associated object, will be passed to the service when it is instantiated.
          [Bootstrap.tag]: {
            enabled: true,
            list: [
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
            ]
          }
        }
      }
    },

    WebSocket_Bs_config: {
      // addresses: {
      //   listen: [
      //     '/dns4/wrtc-star1.par.dwebops.pub/tcp/443/wss/p2p-webrtc-star',
      //     '/dns4/wrtc-star2.sjc.dwebops.pub/tcp/443/wss/p2p-webrtc-star'
      //   ]
      // },
      modules: {
        transport: [Websockets],
        connEncryption: [NOISE],
        streamMuxer: [Mplex],
        peerDiscovery: [Bootstrap]
      },
      config: {
        peerDiscovery: {
          autoDial: true,
          [Bootstrap.tag]: {
            enabled: true,
            list: [
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
              '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
            ]
          }
        }
      }
    }

  }
}

