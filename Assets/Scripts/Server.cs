using UnityEngine;

public class Server : MonoBehaviour {

    public int port = 32154;
    public int maxConnections = 4;
    public GameObject shipPrefab;

    private const string TYPE_NAME = "IHA-SPG0";
    private const string GAME_NAME = "SpaceCrusher Game";
    private string inmessages = "";

    void StartServer() {
        Network.InitializeServer(maxConnections, port, false);
        MasterServer.RegisterHost(TYPE_NAME, GAME_NAME);
    }

    void StopServer() {
        Network.Disconnect();
    }

    //void OnPlayerConnected(NetworkPlayer player) {
    //    this.AddShip();
    //}
    //private void AddShip() {
    //    Network.Instantiate(shipPrefab, new Vector3(333, 444, 0), Quaternion.identity, 0);
    //}

    [RPC]
    void RPCOut(string info) {
        networkView.RPC("RPCIn", RPCMode.Others, info);
    }

    [RPC]
    void RPCIn(string info) {
        Debug.Log("Received message -> " + info);
        inmessages += info + "\n";
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
                if (GUILayout.Button("Make magic")) {
                    RPCOut("isahtoathas");
                }
                GUILayout.Label(inmessages);
            }
            if (GUILayout.Button("Stop Server")) {
                StopServer();
            }
        }
    }
}
