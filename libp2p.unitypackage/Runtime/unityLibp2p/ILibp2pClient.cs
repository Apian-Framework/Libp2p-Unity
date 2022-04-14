using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibp2p
{
    public  interface ILibp2pClient
    {
        void OnCreated(Libp2pPeerId localPeerId); // TODO Create a class for a libp2p peer
        void OnStarted(); // args?
        void OnListenAddress(List<string> addresses);
        void OnPeerDiscovery(string peerId); // TODO: maddrs?
        void OnConnectionEvent(string peerId, bool connected);
        void OnMessage(string sourceId, string channel, string payload);
    }
}