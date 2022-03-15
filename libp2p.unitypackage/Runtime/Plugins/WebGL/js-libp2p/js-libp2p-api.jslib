
// The idea here is that all C#/Javascript interop in both directions (emscripten stuff) is dealt
// with here and that "JsLibp2p" is defined and contains the non-interop-related javascript code for
// actually talking to the js-libp2p library.

var JsLibp2pAPI = {

   // this __postset writes the func pointer declarations when emscripten processes this library module
   JsLibp2p_InitCallbacks__postset: 'var p_OnLibCreated_Cb; var p_OnLibStarted_Cb;  var p_OnDiscovery_Cb; var p_OnConnection_Cb; var p_OnMessageCb;',
   JsLibp2p_InitCallbacks: function( Unity_OnLibCreated_Cb, Unity_LibStarted_Cb, Unity_OnDiscovery_Cb,
      Unity_OnConnection_Cb, Unity_OnMessage_Cb  )
   {
      // Should get called at app start
      p_OnLibCreated_Cb = Unity_OnLibCreated_Cb
      p_OnLibStarted_Cb = Unity_LibStarted_Cb
      p_OnDiscovery_Cb = Unity_OnDiscovery_Cb
      p_OnConnection_Cb = Unity_OnConnection_Cb
      p_OnMessageCb = Unity_OnMessage_Cb

      // NOTE: I thusfar have not been able to figure out how to get the code in JsLibP2p to be able to see
      // the JsLibp2pAPI callbacks to C#, so I'm just passing refs to them during init.
      JsLibp2p.InitApiCallbacks( _JsLibp2p_OnLibCreated, _JsLibp2p_OnLibStarted, _JsLibp2p_OnDiscovery,
         _JsLibp2p_OnConnection, _JsLibp2p_OnMessage)
   },

   // Calls from C#
   JsLibp2p_CreateFromNamedCfg : async function(c_clientId, c_cfgName, c_cfgOpts)
   {
      console.log(`JsLibp2p_CreateLib() Type of passed-in clientID: ${typeof(c_clientId)} `)
      var clientId = UTF8ToString(c_clientId)
      var cfgName = UTF8ToString(c_cfgName)
      var cfgOpts = UTF8ToString(c_cfgOpts)
	   JsLibp2p.CreateFromNamedCfg(clientId, cfgName, cfgOpts)  // don't await - there's a callback
   },

   JsLibp2p_StartLib : async function(c_clientId)
   {
      console.log(`JsLibp2p_StartLib() `)
      var clientId = UTF8ToString(c_clientId)
	   JsLibp2p.StartLib(clientId)  // don't await - there's a callback
   },

   // callbacks from Javascript
   JsLibp2p_OnLibCreated: function(clientId)
   {
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL) // TODO: consider using emcripten stack mem utils?
      dynCall_vi( p_OnLibCreated_Cb, c_clientId)
      _free(c_clientId)
   },

   JsLibp2p_OnLibStarted: function(clientId)
   {
      var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
      dynCall_vi( p_OnLibStarted_Cb, c_clientId)
      _free(c_clientId)
   },

   JsLibp2p_OnDiscovery : function(clientId, peerId)
   {
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var c_peerId = allocate(intArrayFromString(peerId), ALLOC_NORMAL)
       dynCall_vii( p_OnDiscovery_Cb, c_clientId, c_peerId)
       _free(c_clientId)
       _free(c_peerId)
   },

   JsLibp2p_OnConnection : function(clientId, peerId, isConnected) // called by JS so params are ok
   {
       //console.log(`*** About to call Unity connection callback`)
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var c_peerId = allocate(intArrayFromString(peerId), ALLOC_NORMAL)
       dynCall_viii( p_OnConnection_Cb, c_clientId, c_peerId, isConnected)
       _free(c_clientId)
       _free(c_peerId)
   },

   JsLibp2p_OnMessage : function(clientId, sourceId, msgStr)
   {
       //console.log(`*** About to call Unity message callback`)
       var c_clientId = allocate(intArrayFromString(clientId), ALLOC_NORMAL)
       var c_sourceId = allocate(intArrayFromString(peerId), ALLOC_NORMAL)
       var c_msgStr = allocate(intArrayFromString(msgStr), ALLOC_NORMAL)
       dynCall_viii( p_OnMessageCb, c_clientId, c_sourceId, c_msgStr)
       _free(c_clientId)
       _free(c_sourceId)
       _free(c_msgStr)
   },

 }

 mergeInto(LibraryManager.library,JsLibp2pAPI);
