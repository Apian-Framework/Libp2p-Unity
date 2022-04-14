
import {Libp2p_configs, ProcessConfig} from './js-libp2p-configs.js'

const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;

var JslibApiCallbacks = { }

var ClientContexts = { }

function EquivSets(setA, setB)
{
  return ( setA.size == setB.size
    && [...setA].every(function (element) {
      return setB.has(element);
    })
  )
}

export default  {

  InitApiCallbacks: function( OnLibCreated_Cb, OnLibStarted_Cb,  OnDiscovery_Cb,  OnConnection_Cb,
      OnListenAddr_Cb, OnMessage_Cb )
  {
    JslibApiCallbacks.OnLibCreated = OnLibCreated_Cb
    JslibApiCallbacks.OnLibStarted = OnLibStarted_Cb
    JslibApiCallbacks.OnDiscovery = OnDiscovery_Cb
    JslibApiCallbacks.OnConnection = OnConnection_Cb
    JslibApiCallbacks.OnListenAddress = OnListenAddr_Cb,
    JslibApiCallbacks.OnMessage = OnMessage_Cb
  },

  CreateFromNamedCfg : async function(clientId, cfgName, cfgOpts)
  {
    this.Create(clientId, Libp2p_configs[cfgName], cfgOpts);
  },

  CreateFromConfig : async function(clientId, configIn)
  {
    var fullConfig = ProcessConfig(configIn)
    this.Create(clientId, fullConfig);
  },

  Create : async function(clientId, config)
  {
    console.log(`CreateLib(): Type of passed-in clientID: ${typeof(clientId)} `)

    var libp2pInst = await Libp2p.create( config )

    var ClientContext =  {
      version: 2,
      clientId: clientId,
      libp2pInst: libp2pInst,
      subscribedTopics: [],
      listenMaddrs: new Set()
    }
    ClientContexts[clientId] = ClientContext;
		console.log(`Created Libp2p instance: ${clientId} LoalPeerId: ${libp2pInst.peerId.toB58String()}`)

    JslibApiCallbacks.OnLibCreated(clientId, libp2pInst.peerId);
  },

  StartLib : async function(clientId)
  {
      var ctx = await ClientContexts[clientId];

      // Listen for discovered peers
      ctx.libp2pInst.on('peer:discovery', (peer) => {
        const peerIdStr = peer.toB58String();
        console.log('Discovered %s', peerIdStr) // Log discovered peer
        JslibApiCallbacks.OnDiscovery(clientId, peerIdStr)
      })

      // Listen for new connections to peers
      ctx.libp2pInst.connectionManager.on('peer:connect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String();
        console.log(`Inst ${clientId} Connected to ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, peerIdStr, true)
      })

      // Listen for peers disconnecting
      ctx.libp2pInst.connectionManager.on('peer:disconnect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String()
        console.log(`Disconnected from ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, peerIdStr,false)
      })

      // If we bind to a relay or sentinel in order to be abel to have a "listen" address,
      // this will fire with the peerId being our local peerid, and we need to report this
      // back to the client app to tell it what our "listen" address(es) is/are
      ctx.libp2pInst.peerStore.on('change:multiaddrs', ({ peerId }) => {
        // Updated multiaddrs for this local node?
        if (peerId.equals(ctx.libp2pInst.peerId)) {
          var listenAddrs = new Set( ctx.libp2pInst.multiaddrs.map( maddr => maddr.toString()+'/p2p/'+peerId.toB58String() ) )
          // This will get called when the addresses are still the same, so make sure they've changed
          if ( !EquivSets(ctx.listenMaddrs, listenAddrs)) {
            ctx.listenMaddrs = listenAddrs;

            console.log(`Advertising listen addresses of ${listenAddrs}`)
            JslibApiCallbacks.OnListenAddress(clientId,listenAddrs)
          }

        }
      })

      await ctx.libp2pInst.start()

      JslibApiCallbacks.OnLibStarted(clientId);

    },

 }
