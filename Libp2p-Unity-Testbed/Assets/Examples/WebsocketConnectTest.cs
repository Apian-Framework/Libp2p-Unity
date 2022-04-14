using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityLibp2p;

public class WebsocketConnectTest : MonoBehaviour, ILibp2pClient
{
    public TMP_InputField BootStrapFld;
    public TMP_InputField OutputFld; // is set to non-interactive

    public ILibp2p lib;

    // Start is called before the first frame update
    void Start()
    {
        BootStrapFld.text = JsonConvert.SerializeObject(Libp2pConfig.ExampleWebsocketsConfig, Formatting.Indented);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DoConnect()
    {
        Log("Connect pressed.");

        object configObj = JsonConvert.DeserializeObject(BootStrapFld.text);

        lib = Libp2p.Factory(this, configObj);
        // WebSocket_Bs_config
        // WebRtcStar_Bs_Gossip_config
    }

    public void Log(string msg)
    {
        OutputFld.text = $"{OutputFld.text}\n{msg}";
    }

    // ILibp2pClient implementation
    public void OnCreated(Libp2pPeerId localPeer)
    {
        Log( $"Lib instance {lib.InstanceId} created.");
        Log( $"LocalPeer: {(localPeer.ToString(true))}");

        if (!lib.IsStarted)
            lib.Start();
    }
    public void OnStarted() {}
    public void OnListenAddress(List<string> addresses) {}
    public void OnPeerDiscovery(string peerId) {}
    public void OnConnectionEvent(string peerId, bool connected) {}
    public void OnMessage(string sourceId, string channel, string payload) {}




}
