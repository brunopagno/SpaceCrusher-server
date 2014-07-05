using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour {

    public int port = 32154;
    public int maxConnections = 4;
    public PlayerShip shipPrefab;

    private int PLAYER_ID = 1;
    private List<PlayerShip> ships = new List<PlayerShip>(); 

    private const string TYPE_NAME = "IHA-SPG0";
    private const string GAME_NAME = "SpaceCrusher Game";
    private string inmessages = "";

    void StartServer() {
        Network.InitializeServer(maxConnections, port, false);
        MasterServer.RegisterHost(TYPE_NAME, GAME_NAME);
    }

    void StopServer() {
        Network.Disconnect();
        foreach (PlayerShip ship in ships) {
            ship.RemoveFromGame();
        }
        ships.Clear();
    }

    void OnPlayerConnected(NetworkPlayer player) {
        PlayerShip ship = (PlayerShip) Instantiate(shipPrefab, new Vector3(1, 2.5f, 0), Quaternion.identity);
        ship.Id = NextPlayerId();
        ship.Player = player;
        ships.Add(ship);
        networkView.RPC("RPCIn", player, "PID:" + ship.Id);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
        for (int i = 0; i < ships.Count; i++) {
            PlayerShip ship = ships[i];
            if (ship.Player.Equals(player)) {
                ship.RemoveFromGame();
                ships.RemoveAt(i);
                break;
            }
        }
    }

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

    private int NextPlayerId() {
        return PLAYER_ID++;
    }
}
