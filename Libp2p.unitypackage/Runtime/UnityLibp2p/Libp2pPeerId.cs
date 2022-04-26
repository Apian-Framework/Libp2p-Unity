using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityLibp2p
{
    public class Libp2pPeerId
    {
        public string id {get; private set; }
        public string pubKey {get; private set; }
        public string privKey { get; private set; }  // TODO: this is not at all secure

        public Libp2pPeerId(string _id, string _pubKey, string _privKey = null)
        {
            id = _id;
            pubKey = _pubKey;
            privKey = _privKey;
        }

        public string ToString(bool prettyPrint=false) =>  JsonConvert.SerializeObject(this, prettyPrint ?  Formatting.Indented : Formatting.None);

        public static Libp2pPeerId FromObject(object peerObj)
        {
            return new Libp2pPeerId(
                (string)peerObj.GetType().GetProperty("id")?.GetValue(peerObj),
                (string)peerObj.GetType().GetProperty("pubKey")?.GetValue(peerObj),
                (string)peerObj.GetType().GetProperty("privKey")?.GetValue(peerObj) );

        }

        public static Libp2pPeerId FromDict(Dictionary<string, string> peerDict)
        {
            return new Libp2pPeerId(
                peerDict["id"],
                peerDict.ContainsKey("pubKey") ? peerDict["pubKey"] : null,
                peerDict.ContainsKey("privKey") ? peerDict["privKey"]: null );
        }

    }

}