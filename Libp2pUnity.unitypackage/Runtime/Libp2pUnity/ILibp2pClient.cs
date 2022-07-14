using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Libp2pUnity
{
    public  interface ILibp2pClient
    {
        void OnCreated(Libp2pPeerId localPeerId); // TODO Create a class for a libp2p peer
        void OnStarted();
        void OnStopped();
        void OnListenAddress(List<string> addresses);
        void OnPeerDiscovery(Libp2pPeerId peerId); // TODO: maddrs?
        void OnPing(string peerAddr, int latencyMs);
        void OnConnectionEvent(Libp2pPeerId peerId, bool connected);
        void OnMessage(string sourceId, string topic, string payload);
    }
}