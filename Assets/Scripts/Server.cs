using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class Server : MonoBehaviour {

    private Thread serverThread;

    private UdpClient client;

    public int port = 3339;

    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = "";

    void OnGUI() {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "Last Packet: \n" + lastReceivedUDPPacket
                    + "\n\nAll Messages: \n" + allReceivedUDPPackets, style);
    }

	void Start () {
         print("UDPSend.init()");
 
        serverThread = new Thread(new ThreadStart(Listen));
        serverThread.IsBackground = true;
        serverThread.Start();
	}

    void Listen() {
        Debug.Log("Server starting on separate thread");
        client = new UdpClient(port);
        while (true) {
            try {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                Debug.Log("Cliente enviou bagaça. ip:" + anyIP.Address + " port:" + anyIP.Port);
                string text = Encoding.UTF8.GetString(data);
                print(">> " + text);
                lastReceivedUDPPacket = text;
                allReceivedUDPPackets = allReceivedUDPPackets+text;
            } catch (Exception err) {
                print(err.ToString());
            }
        }
    }

    void OnApplicationQuit() {
        Debug.Log("Quit server on application quit");
        this.serverThread.Abort();
        if (this.client != null)
            this.client.Close();
    }
}
