using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibp2p
{

    public class Libp2p
    {

        // Factory
        public static ILibp2p Factory(ILibp2pClient client, Libp2pConfig config)
        {
            // assumes there is a named configuration "configName" in the javascript code
#if  UNITY_WEBGL && !UNITY_EDITOR
            return  WebGLLibp2p.Factory(client, config);
#else
           throw new NotImplementedException("Only WEBGL supported");
#endif

        }


    }

}