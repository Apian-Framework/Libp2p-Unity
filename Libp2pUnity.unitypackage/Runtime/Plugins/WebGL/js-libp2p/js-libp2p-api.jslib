
// The idea here is that all C#/Javascript interop in both directions (emscripten stuff) is dealt
// with here and that "JsLibp2p" is defined and contains the non-interop-related javascript code for
// actually talking to the js-libp2p library.

var JsLibp2pAPI = {

   // this __postset writes the func pointer declarations when emscripten processes this library module
   JsLibp2p_InitCallbacks__postset: 'var p_OnLibCreated_Cb; var p_OnLibStarted_Cb; var p_OnDiscovery_Cb; var p_OnConnection_Cb; var p_OnListenAddr_Cb; var p_OnMessageCb; var p_OnLibStoppedCb; var p_OnPing_Cb;',
   JsLibp2p_InitCallbacks: function( Unity_OnLibCreated_Cb, Unity_LibStarted_Cb, Unity_OnDiscovery_Cb,
      Unity_OnConnection_Cb, Unity_OnListenAddr_Cb, Unity_OnMessage_Cb, Unity_OnLibStopped_Cb, Unity_OnPing_Cb )
   {
      // Should get called at app start
      p_OnLibCreated_Cb = Unity_OnLibCreated_Cb
      p_OnLibStarted_Cb = Unity_LibStarted_Cb
      p_OnDiscovery_Cb = Unity_OnDiscovery_Cb
      p_OnConnection_Cb = Unity_OnConnection_Cb
      p_OnListenAddr_Cb = Unity_OnListenAddr_Cb
      p_OnMessageCb = Unity_OnMessage_Cb
      p_OnLibStoppedCb = Unity_OnLibStopped_Cb
      p_OnPing_Cb = Unity_OnPing_Cb

      // NOTE: I thusfar have not been able to figure out how to get the code in JsLibP2p to be able to see
      // the JsLibp2pAPI callbacks to C#, so I'm just passing refs to them during init.
      JsLibp2p.InitApiCallbacks( _JsLibp2p_OnLibCreated, _JsLibp2p_OnLibStarted, _JsLibp2p_OnDiscovery,
         _JsLibp2p_OnConnection, _JsLibp2p_OnListenAddress, _JsLibp2p_OnMessage, _JsLibp2p_OnLibStopped, _JsLibp2p_OnPing )
   },

   // Calls from C#
   JsLibp2p_CreateFromNamedCfg: async function(c_clientId, c_cfgName, c_cfgOpts)
   {
      var clientId = UTF8ToString(c_clientId)
      var cfgName = UTF8ToString(c_cfgName)
      var cfgOpts = UTF8ToString(c_cfgOpts)
	   JsLibp2p.CreateFromNamedCfg(clientId, cfgName, cfgOpts)  // don't await - there's a callback
   },

   JsLibp2p_CreateFromConfig: async function(c_clientId, c_configJson)
   {
      var clientId = UTF8ToString(c_clientId)
      var config = JSON.parse(UTF8ToString(c_configJson))
      JsLibp2p.CreateFromConfig(clientId, config)
   },

   JsLibp2p_StartLib: async function(c_clientId)
   {
      console.log(`JsLibp2p_StartLib() `)
      var clientId = UTF8ToString(c_clientId)
      JsLibp2p.StartLib(clientId)  // don't await - there's a callback
   },

   JsLibp2p_StopLib: async function(c_clientId)
   {
      console.log(`JsLibp2p_StopLib() `)
      var clientId = UTF8ToString(c_clientId)
	   JsLibp2p.StopLib(clientId)
   },

   JsLibp2p_Subscribe:  function(c_clientId, c_topic)
   {
      // returns a bool
      console.log(`JsLibp2p_Subscribe()`)
      var clientId = UTF8ToString(c_clientId)
      var topic = UTF8ToString(c_topic)
	   var success = JsLibp2p.Subscribe(clientId, topic)
      console.log(`JsLibp2p_Subscribe() ${success?"succeeded":"failed"}`)
      return success
   },

   JsLibp2p_Unsubscribe: function(c_clientId, c_topic)
   {
      // returns a bool
      var clientId = UTF8ToString(c_clientId)
      var topic = UTF8ToString(c_topic)
	   var success =  JsLibp2p.Unsubscribe(clientId, topic)
      console.log(`JsLibp2p_Unsubscribe() ${success?"succeeded":"failed"}`)
      return success
   },

   JsLibp2p_Publish: function(c_clientId, c_topic, c_message)
   {
      var clientId = UTF8ToString(c_clientId)
      var topic = UTF8ToString(c_topic)
      var message = UTF8ToString(c_message)
      JsLibp2p.Publish(clientId, topic, message)  // calls await, be we aren't waiting (TODO: should we?)
      console.log(`JsLibp2p_Publish(${topic}, ${message})`)
   },

   JsLibp2p_Dial: function(c_clientId, c_peerAddr)
   {
      var clientId = UTF8ToString(c_clientId)
      var peerAddr = UTF8ToString(c_peerAddr)
      JsLibp2p.Dial(clientId, peerAddr)
      console.log(`JsLibp2p_Dial(${peerAddr})`)
   },

   JsLibp2p_HangUp: function(c_clientId, c_peerAddr)
   {
      var clientId = UTF8ToString(c_clientId)
      var peerAddr = UTF8ToString(c_peerAddr)
      JsLibp2p.HangUp(clientId, peerAddr)
      console.log(`JsLibp2p_HangUp(${peerAddr})`)
   },

   JsLibp2p_Ping: function(c_clientId, c_peerAddr)
   {
      var clientId = UTF8ToString(c_clientId)
      var peerAddr = UTF8ToString(c_peerAddr)
      JsLibp2p.Ping(clientId, peerAddr)
      console.log(`JsLibp2p_Ping(${peerAddr})`)
   },

   // callbacks from Javascript
   JsLibp2p_OnLibCreated: function(clientId, peerId)
   {
      var peerIdJson = JSON.stringify(peerId)
      var c_peerIdJson = allocate(intArrayFromString(peerIdJson), ALLOC_NORMAL)
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL) // TODO: consider using emcripten stack mem utils?

      dynCall_vii( p_OnLibCreated_Cb, c_clientId, c_peerIdJson)
      _free(c_clientId)
      _free(c_peerIdJson)
   },

   JsLibp2p_OnLibStarted: function(clientId)
   {
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
      dynCall_vi( p_OnLibStarted_Cb, c_clientId)
      _free(c_clientId)
   },

   JsLibp2p_OnDiscovery: function(clientId, peerId)
   {
      var peerIdJson = JSON.stringify(peerId)
      var c_peerIdJson = allocate(intArrayFromString(peerIdJson), ALLOC_NORMAL)
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
      dynCall_vii( p_OnDiscovery_Cb, c_clientId, c_peerIdJson)
      _free(c_clientId)
      _free(c_peerIdJson)
   },

   JsLibp2p_OnConnection: function(clientId, peerId, isConnected) // called by JS so params are ok
   {
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var peerIdJson = JSON.stringify(peerId)
       var c_peerIdJson = allocate(intArrayFromString(peerIdJson), ALLOC_NORMAL)

       dynCall_viii( p_OnConnection_Cb, c_clientId, c_peerIdJson, isConnected)
       _free(c_clientId)
       _free(c_peerIdJson)
   },

   JsLibp2p_OnListenAddress: function(clientId, listenAddrArray) // p2pLib has announced a "local" listen (via realy) address(es)
   {
      var listenAddrArrayJson = JSON.stringify(listenAddrArray)
      var c_listenAddrJson = allocate(intArrayFromString(listenAddrArrayJson), ALLOC_NORMAL)
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
      dynCall_vii( p_OnListenAddr_Cb, c_clientId, c_listenAddrJson)
      _free(c_clientId)
      _free(c_listenAddrJson)
   },

   JsLibp2p_OnMessage: function(clientId, sourceId, topic, msgStr)
   {
       console.log(`*** About to call Unity message callback`)
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var c_sourceId = allocate(intArrayFromString(sourceId), ALLOC_NORMAL)
       var c_topic = allocate(intArrayFromString(topic), ALLOC_NORMAL)
       var c_msgStr = allocate(intArrayFromString(msgStr), ALLOC_NORMAL)
       dynCall_viiii( p_OnMessageCb, c_clientId, c_sourceId, c_topic, c_msgStr)
       _free(c_clientId)
       _free(c_sourceId)
       _free(c_topic)
       _free(c_msgStr)
   },

   JsLibp2p_OnLibStopped: function(clientId)
   {
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
      dynCall_vi( p_OnLibStoppedCb, c_clientId)
      _free(c_clientId)
   },

   JsLibp2p_OnPing: function(clientId, remoteAddr, latencyMs)
   {
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var c_remoteAddr = allocate(intArrayFromString(remoteAddr), ALLOC_NORMAL)

       dynCall_viii( p_OnPing_Cb, c_clientId, c_remoteAddr, latencyMs)
       _free(c_clientId)
       _free(c_remoteAddr)

   },

 }

 mergeInto(LibraryManager.library,JsLibp2pAPI);
