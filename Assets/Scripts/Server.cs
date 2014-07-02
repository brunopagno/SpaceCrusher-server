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
    private IPEndPoint remoteIpEndPoint;

    public int port = 3339;

	// Use this for initialization
	void Start () {
        client = new UdpClient(this.port);
        remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        serverThread = new Thread(new ThreadStart(Listen));
        serverThread.IsBackground = true;
        serverThread.Start();
	}

    void Listen() {
        Debug.Log("Server starting on separate thread");
        while(true) {
            try {
                Debug.Log("Waiting to reveice data");
                Byte[] receiveBytes = this.client.Receive(ref remoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                Debug.Log("This is the message you received " + returnData.ToString());
                Debug.Log("This message was sent from " + remoteIpEndPoint.Address.ToString() +
                                            " on their port number " + remoteIpEndPoint.Port.ToString());
            } catch (Exception e) {
                Debug.Log(e.ToString());
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
