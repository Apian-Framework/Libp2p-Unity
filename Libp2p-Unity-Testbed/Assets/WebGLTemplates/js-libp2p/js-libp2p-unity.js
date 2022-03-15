
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
      var ctx = ClientContexts[clientId]
      var libp2pInst = ctx.libp2pInst

      // Listen for discovered peers
      libp2pInst.on('peer:discovery', (peer) => {
        const peerIdStr = connection.remotePeer.toB58String();
        console.log('Discovered %s', peerIdStr) // Log discovered peer
        JslibApiCallbacks.OnDiscovery(clientId, peerIdStr)
      })

      // Listen for new connections to peers
      libp2pInst.connectionManager.on('peer:connect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String();
        console.log(`Inst ${clientId} Connected to ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, peerIdStr, true)
      })

      // Listen for peers disconnecting
      libp2pInst.connectionManager.on('peer:disconnect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String();
        console.log(`Disconnected from ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, peerIdStr,false)
      })

      libp2pInst.peerStore.on('change:multiaddrs', ({ peerId }) => {
        // How is it that node.multiaddrs can be empty at thsi point? But it can be.
        if (libp2pInst.multiaddrs.length === 0) {
          console.log(`on(change:multiaddrs): libp2pInst.multiaddrs[] is EMPTY!`)
        } else {
          var multiStr = libp2pInst.multiaddrs[0].toString()
          console.log(`on(change:multiaddrs):: ${multiStr}/p2p/${libp2pInst.peerId.toB58String()}`)
          // Updated self multiaddrs?
          if (peerId.equals(libp2pInst.peerId)) {
            console.log(`Advertising with a relay address of ${multiStr}/p2p/${peerId.toB58String()}`)
          }
        }
      })

      try {
        await libp2pInst.start()
      } catch (err) {
        console.log('start failed with:', err.message)
      }

      //console.log('dialing...')

      //try {
         //await ctx.libp2pInst.dial('/ip4/64.227.18.159/tcp/15003/ws/p2p/Qmcv4ZGnuSBeD36iMLqUKpNP5BaUmDq6YZtJmp5b5ozXCP')
      //   var conn = await ctx.libp2pInst.dial('/ip4/64.227.18.159/tcp/15003/ws/p2p/Qmcv4ZGnuSBeD36iMLqUKpNP5BaUmDq6YZtJmp5b5ozXCP/p2p-circuit/p2p/QmP9tutwqGJHAcpn6vKMFEaqUSXCJnsiNHVt1KfrndthBu')
      //} catch (err) {
      //   console.log('dial failed with', err.message)
      //}


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
