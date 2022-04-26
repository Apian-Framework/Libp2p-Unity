using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibp2p
{
    // Creation is done via
    // static ILibp2p <Implementation>.Factory(ILibp2pClient clientInst, object libp2pConfigObj)

    public  interface ILibp2p
    {
        string InstanceId {get;}
        bool IsStarted {get;}
        Libp2pPeerId PeerId {get;}
        List<string> ListenAddrs {get;}

        void Start();
        void Stop();
        bool Subscribe(string topic);
        bool Unsubscribe(string topic);
        void Publish(string topic, string payload);
        void Dial( string address);
        void HangUp( string address);
        void Ping( string address);
    }


}