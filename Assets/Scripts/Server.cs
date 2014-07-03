using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class Server : MonoBehaviour {

    public int port = 32154;
    public int maxConnections = 4;

    void StartServer() {
        Debug.Log("Server start");
        Network.InitializeServer(maxConnections, port, false);
    }

    void StopServer() {
        Debug.Log("Server stop");
        Network.Disconnect();
    }

    void OnGUI() {
        if (Network.peerType == NetworkPeerType.Disconnected) {
            GUILayout.Label("Game server Offline");
            if (GUILayout.Button("Start Game Server")) {
                StartServer();
            }
        } else {
            if (Network.peerType == NetworkPeerType.Connecting) {
                GUILayout.Label("Server Starting");
            } else {
                GUILayout.Label("Game Server Online");
                GUILayout.Label("Server Ip: " + Network.player.ipAddress + " Port: " + Network.player.port);
                GUILayout.Label("Clients: " + Network.connections.Length + "/" + maxConnections);

                foreach (NetworkPlayer client in Network.connections) {
                    GUILayout.Label("Client " + client);
                }
            }
            if (GUILayout.Button("Stop Server")) {
                StopServer();
            }
        }
    }
}
