using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityLibp2p;

public class WebsocketConnectTest : MonoBehaviour, ILibp2pClient
{
    public TMP_InputField ConfigFld;
    public TMP_InputField TopicFld;
    public TMP_InputField MsgFld;
    public TMP_InputField OutputFld; // is set to non-interactive
    public TMP_Dropdown TopicSel;

    public TMP_InputField PeerAddrFld;

    protected UnityEngine.UI.Scrollbar outScrollBar;

    protected string SelectedTopic => TopicSel.options[TopicSel.value].text;

    public ILibp2p lib;

    // Start is called before the first frame update
    void Start()
    {
        ConfigFld.text = JsonConvert.SerializeObject(Libp2pConfig.ExampleWebsocketsConfig, Formatting.Indented);
        outScrollBar = OutputFld.transform.Find("Scrollbar").gameObject.GetComponent<UnityEngine.UI.Scrollbar>();
        TopicSel.ClearOptions();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DoConnect()
    {
        Log("Connect pressed.");

        object configObj = JsonConvert.DeserializeObject(ConfigFld.text);

        lib = Libp2p.Factory(this, configObj);
     }

    public void DoDisconnect()
    {
        Log("Disconnect pressed.");
        lib?.Stop();
    }

    public void DoSubscribe()
    {
        string topic = TopicFld.text;
        if ( lib.Subscribe(topic) )
        {
            List<string> topicList = new List<string>() {topic};
            TopicSel.AddOptions( topicList );
            Log($"Subscribed to {topic}");
        } else {
            Log($"Failed to subscrib to {topic}");
        }
    }

    public void DoUnsubscribe()
    {
        string topic = TopicFld.text;
        if ( lib.Unsubscribe(topic) )
        {
            List<string> newTopics = TopicSel.options.Where(t => t.text != topic).Select(t => t.text ).ToList();
            TopicSel.ClearOptions();
            TopicSel.AddOptions( newTopics );

            Log($"Unsubscribed from {topic}");
        } else {
          Log($"Failed to unsubscribe from {topic}");
        }
    }

    public void DoPublish()
    {
        string topic = SelectedTopic;
        string payload = MsgFld.text;
        Log($"Publishing to {topic}: {payload}");
        lib.Publish(topic, payload);
    }

    public void DoDial()
    {
        string addr = PeerAddrFld.text;
        Log($"Dialing {addr}");
        lib.Dial(addr);
    }

    public void DoHangup()
    {
        string addr = PeerAddrFld.text;
        Log($"Hanging up on {addr}");
        lib.HangUp(addr);
    }

    public void DoPing()
    {
        string addr = PeerAddrFld.text;
        Log($"Pinging {addr}");
        lib.Ping(addr);
    }

    public void Log(string msg)
    {
        OutputFld.text = $"{OutputFld.text}{msg}\n";
        outScrollBar.value = 1; // scroll to end on log()
    }

    // ILibp2pClient implementation
    public void OnCreated(Libp2pPeerId localPeer)
    {
        Log( $"\nLib instance {lib.InstanceId} created.");
        Log( $"\nLocalPeer: {(localPeer.ToString(true))}");

        if (!lib.IsStarted)
            lib.Start();
    }
    public void OnStarted()
    {
        Log( $"\nLibp2p instance started.");
    }

    public void OnStopped()
    {
        Log( $"\nLibp2p instance {lib.InstanceId} stopped.");
    }

    public void OnListenAddress(List<string> addresses)
    {
        foreach( string maddr in addresses )
        {
            //Log($"\nListening on: {maddr}");

            Log($"\nListening\u00A0on:\u00A0{maddr} "); // non-breaking spaces. Otherwise it breaks at the spaces. The address is always too long.
            // and I'd rather it just print it all in one wrapped line.
        }
        Log(""); // blakn line
    }
    public void OnPeerDiscovery(Libp2pPeerId peerId)
    {
        Log( $"\nRemote peer discovered: {peerId.id}");
    }
    public void OnConnectionEvent(Libp2pPeerId peerId, bool connected)
    {
        Log( $"\n{(connected ? "Connected to" : "Disconnected from")}  remote peer: {peerId.id}");
    }
    public void OnMessage(string sourceId, string topic, string payload)
    {
        Log( $"Message from {sourceId} on topic \"{topic}\": \"{payload}\"");
    }
    public void OnPing(string peerAddr, int latencyMs)
    {
        Log( $"Ping returned from {peerAddr} - latency: {latencyMs} ms");
    }



}
