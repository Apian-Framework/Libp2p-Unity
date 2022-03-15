
import {Libp2p_configs} from './js-libp2p-configs.js'

const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,
    Bootstrap,  GossipSub,  FloodSub, fromString, toString }  = window.Libp2pObj;

var JslibApiCallbacks = { }

var ClientContexts = { }

export default  {

  InitApiCallbacks: function( OnLibCreated_Cb, OnLibStarted_Cb, OnDiscovery_Cb,
      OnConnection_Cb, OnMessage_Cb )
  {
    JslibApiCallbacks.OnLibCreated = OnLibCreated_Cb
    JslibApiCallbacks.OnLibStarted = OnLibStarted_Cb
    JslibApiCallbacks.OnDiscovery = OnDiscovery_Cb
    JslibApiCallbacks.OnConnection = OnConnection_Cb
    JslibApiCallbacks.OnMessage = OnMessage_Cb
  },

  CreateFromNamedCfg : async function(clientId, cfgName, cfgOpts)
  {
    this.Create(clientId, Libp2p_configs[cfgName], cfgOpts);
  },

  Create : async function(clientId, config)
  {
    console.log(`CreateLib(): Type of passed-in clientID: ${typeof(clientId)} `)

    var libp2pInst = await Libp2p.create( config )

    var ClientContext =  {
      version: 2,
      clientId: clientId,
      libp2pInst: libp2pInst,
      subscribedTopics: []
    }
    ClientContexts[clientId] = ClientContext;
		console.log(`**** Created Libp2p instance!!!`)

    JslibApiCallbacks.OnLibCreated(clientId);
  },

  StartLib : async function(clientId)
  {
      var ctx = await ClientContexts[clientId];

      // Listen for discovered peers
      ctx.libp2pInst.on('peer:discovery', (peer) => {
        const peerIdStr = connection.remotePeer.toB58String();
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
        const peerIdStr = connection.remotePeer.toB58String();
        console.log(`Disconnected from ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, peerIdStr,false)
      })

      await ctx.libp2pInst.start()
      console.log(`**** Libp2p instance ${clientId} started!!!`)

      JslibApiCallbacks.OnLibStarted(clientId);

    },

    // // callbacks from Javascript
    // P2pNetJsLibp2p_OnConnection : function(clientId, peer) // called by JS so params are ok
    // {
    //     //console.log(`*** About to call Unity connection callback`)
    //     const ctx = ClientContexts[clientId]
    //     var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL) // consider using emcripten stack mem utils?
    //     var c_peer = allocate(intArrayFromString(peer), ALLOC_NORMAL)
    //     dynCall_vii( ctx.connectionCallback, c_clientId, c_peer)
    //     _free(c_clientId)
    //     _free(c_peer)
    // },
 }
