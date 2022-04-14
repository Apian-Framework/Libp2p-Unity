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
        void Start();
    }


}