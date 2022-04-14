
# LibP2p-Unity Phase 0

The goal for Phase 0 is to implement an API to `js-libp2p` that provides only what is necessary in order to use it as a "carrier protocol" for Apian P2pNet networking. Because Unity builds using `js-libp2p` are transpiled to  WASM and run in a browser, the only `libp2p` network protocols supported are `WebSockets` and `WebRTCStar`

---
## The `P2pNet` network model

P2pNet provides a client application with a simple channel-based publish/subscribe view of networking, with an API that consists of:

- `Connect( IP2pNetClient client, NetworkConnectionString)` Does whatever carrier protocol-specific setup is necessary to be able to communicate with the "network", where network is loosely defined as "the set of peers with which the application needs to communicate." "Client" is the an instance of `IP2pNetClient` which will be sent any messages or notifications. No messages are sent to or received from peers during the connect process. `Connect` is not actually an API method, but is currently implemented as part of the P2pNet instance construction process - but I think this was a bad design choice and should change in the future.

- `Join(mainChannelInfo, localHelloData)` - Given a description of a "main channel" for the network, and a packet of static application-specific data identifying the local peer, create (if necessary) and start listening for messages on both that channel, and another subchannel specifically for direct messages to the local peer. On first hearing of one another peers exchange "HELLO" messages containing the data about themselves. At this point the client's `IP2pNetClient` interface will start receiving messages.

- `AddSubchannel(sucChannelInfo, localChannelHelloData)` - Similar to `Join()` except that it creates/joins/listens to a "subchannel" - in practice just another pubsub channel with its own channel-specific hello data. While all network peers are always subscribed to the "main" channel, subchannels might only have a few subscribers.

- `Send(channelId, payload)` - Publishes the application-specific serialized payload to the provided channel.

- `RemoveSubchannel(subChannelId)` - Sends a "goodbye" message and unsubscribes from the provided subchannel. A peer cannot unsubscribe from the network main channel.

- `Leave()` - Unsubscribe from the all (including main) network channels and reinitialize all peer and channel data.

There are some other, mostly informational, P2pNet API methods but the above list is pretty much it.

---

Specifically, in order to be a P2pNet carrier protocol it is only necessary to be able to support the abstract `CarrierProtocol*` methods of `P2pNetBase`, namely:

```
    void CarrierProtocolPoll();
    void CarrierProtocolJoin(P2pNetChannelInfo mainChannel, string localId, string localHelloData);
    void CarrierProtocolSend(P2pNetMessage msg);
    void CarrierProtocolListen(string channel);
    void CarrierProtocolStopListening(string channel);
    void CarrierProtocolLeave();
    string CarrierProtocolNewP2pId();
    void CarrierProtocolAddReceiptTimestamp(P2pNetMessage msg);
```

# LibP2p-Unity Phase Zero caveats:

First: phase zero by definition exists to support Unity WEBGL (browser) builds, which implies:

- It is using the `js-libp2p` implementation of `Libp2p`
- It only supports the `Websocket` and `WebRTCStar` transport protocols.

And because `P2pNet` is a pubsub-style protocol, phase zero is primarily focused on  pubsub messaging.

In addition, because Unity WEBGL builds cannot use multi-threading, the use of C# `async/await` asynchronous techniques is very limited. As a result, Phase Zero is implmented using callback methods.

 # The API

An application must provide an instance of an `ILibp2pClient` to handle message and status notifications:
```
    public  interface ILibp2pClient
    {
        void OnCreated(Libp2pPeer localPeer);
        void OnStarted();
        void OnListenAddress(List<string> addresses);
        void OnPeerDiscovery(string peerId);
        void OnConnectionEvent(string peerId, bool connected);
        void OnMessage(string sourceId, string channel, string payload);
    }
```

and interaction with the library is through the `ILibp2p` interface:









# Working Notes

LibP2p PubSub does NOT do its own discovery, and so requires discovery to be specified.

??? Should this implementation explicitly _dial_ any peer it hears of/from? Or does libp2p already do this?

*  ConnectionManager.autoDial - should be true
*  Set (for now) everyone to be a relay/router:
    relay: {
      enabled: true,
      hop: {
        enabled: true
      }
    },

Apparently WebRTCStar *does* do some sort of autodiscovery?

NEED JS Unit8Array interop!!!

In the config file the correct keywords are:
- "transport", not "transports" (the docs go both ways)
- "streamMuxer", not "streamMuxers" (once again - docs are inconsistent)

