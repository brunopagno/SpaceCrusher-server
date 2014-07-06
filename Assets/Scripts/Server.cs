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

    private int NextPlayerId() {
        return PLAYER_ID++;
    }
}
