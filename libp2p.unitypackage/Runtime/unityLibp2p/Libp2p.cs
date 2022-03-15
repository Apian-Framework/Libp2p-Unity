using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibp2p
{

    public class Libp2p : ILibp2p
    {

        protected Libp2p()
        {

        }

        // Static API
        public static ILibp2p Create(string configName, string configOptions)
        {
            // assumes there is a named configuration "configName" in the javascript code
#if UNITY_WEBGL && !UNITY_EDITOR
             return  WebGLLibp2p.CreateNamedConfig(configName, configOptions);
#else
            throw new NotImplementedException("Only WEBGL supported");
#endif

        }


//         public static ILibp2p create(Dictionary<string, object> options)
//         {
// #if WEBGL
//             return new UnityWebGLLibp2p(options);
// #endif

//             return null;
//         }

    }

}