using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityLibp2p;

public class WebsocketConnectTest : MonoBehaviour
{
    public TMP_InputField BootStrapFld;
    public TMP_InputField OutputFld; // is set to non-interactive

    public ILibp2p lib;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DoConnect()
    {
        Log("Start DoConnect!!!");

        lib = Libp2p.Create("WebSocket_Bs_config", "cfgOptions");
        // WebSocket_Bs_config
        // WebRtcStar_Bs_Gossip_config

        Log("Start DoConnect!!!");
    }

    public void Log(string msg)
    {
        OutputFld.text = $"{OutputFld.text}\n{msg}";
    }






}
