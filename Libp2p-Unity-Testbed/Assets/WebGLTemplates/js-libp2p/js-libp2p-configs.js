const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;

export var Libp2p_configs = {

  WebRtcStar_Bs_Gossip_config: {
    addresses: {
      // Add the signaling server address, along with our PeerId to our multiaddrs list
      // libp2p will automatically attempt to dial to the signaling server so that it can
      // receive inbound connections from other peers
      listen: [
        '/dns4/newsweasel.com/tcp/443/wss/p2p-webrtc-star/'
        //'/dns4/wrtc-star1.par.dwebops.pub/tcp/443/wss/p2p-webrtc-star',
        //'/dns4/wrtc-star2.sjc.dwebops.pub/tcp/443/wss/p2p-webrtc-star'
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
          enabled: false,
          list: [
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
          ]
        }
      }
    }
  },

  WebSocket_Bs_config: {
    modules: {
      transport: [Websockets],
      connEncryption: [NOISE],
      streamMuxer: [Mplex],
      peerDiscovery: [Bootstrap]
    },
    config: {
      relay: {
        enabled: true,
        autoRelay: {
          enabled: true,
          maxListeners: 2
        }
      },
      peerDiscovery: {
        autoDial: true,
        [Bootstrap.tag]: {
           enabled: true,
           list: [
            '/dns4/newsweasel.com/tcp/15003/wss/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       //'/dnsaddr/newsweasel.com/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       //'/ip4/64.227.18.159/tcp/43237/ws/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       //'/ip4/10.10.0.5/tcp/43237/ws/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
           ]
        }
      }
    }
  }

}

