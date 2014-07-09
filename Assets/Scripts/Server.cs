﻿using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour {

    public int port = 32154;
    public int maxConnections = 4;
    public PlayerShip shipPrefab;
    public GameObject asteroidPrefab;

    private int PLAYER_ID = 1;
    private List<PlayerShip> ships = new List<PlayerShip>();
    private bool isGameStarted;
    public bool IsGameStarted {
        get { return this.isGameStarted; }
        private set { this.isGameStarted = value; }
    }
    private bool isGameEnded = false;
    public bool IsGameEnded {
        get { return this.isGameEnded; }
        set { this.isGameEnded = value; }
    }
    private bool theAsteroidsAreThere = false;

    private const string TYPE_NAME = "IHA-SPG0";
    private const string GAME_NAME = "SpaceCrusher Game";

    void StartServer() {
        Network.InitializeServer(maxConnections, port, false);
        MasterServer.RegisterHost(TYPE_NAME, GAME_NAME);
        IsGameStarted = false;
    }

    void StopServer() {
        Network.Disconnect();
        foreach (PlayerShip ship in ships) {
            ship.RemoveFromGame();
        }
        ships.Clear();
    }

    void OnPlayerConnected(NetworkPlayer player) {
        if (!IsGameStarted) {
            PlayerShip ship = (PlayerShip)Instantiate(shipPrefab);
            ship.dotRenderer.color = NextPlayerColor();
            ship.Id = NextPlayerId();
            ship.Player = player;
            ships.Add(ship);
            networkView.RPC("RPCIn", player, "PID:" + ship.Id);
        }
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

    #region RPC
    [RPC]
    public void RPCOut(string info) {
        networkView.RPC("RPCIn", RPCMode.Others, info);
    }

    [RPC]
    public void SendPosition(string position) { }

    [RPC]
    public void SendChangedGun(string gun) { }

    [RPC]
    void ChangeGun(string message) { }

    [RPC]
    void MovePlayer(string message) {
        string[] d = message.Split(':');
        PlayerShip s = GetShip(int.Parse(d[0]));
        s.MoveTo(float.Parse(d[1]));
    }

    [RPC]
    void RPCIn(string info) {
        Debug.Log("Received message -> " + info);
    }

    [RPC]
    void RPCStart(string nothing) {
        IsGameStarted = true;
        networkView.RPC("RPCStart", RPCMode.Others, string.Empty);

        if (!theAsteroidsAreThere) {
            theAsteroidsAreThere = true;
            for (int i = 0; i < 4; i++) {
                Instantiate(asteroidPrefab, new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0), Quaternion.identity);
            }
        }

        foreach (PlayerShip ship in ships) {
            SetLife(ship.Id + ":" + ship.life);
        }
    }

    [RPC]
    public void SetLife(string message) {
        networkView.RPC("SetLife", RPCMode.Others, message);
    }
    #endregion

    private int NextPlayerId() {
        return PLAYER_ID++;
    }

    private Color NextPlayerColor() {
        switch (PLAYER_ID) {
            case 1:
                return Color.blue;
            case 2:
                return Color.red;
            case 3:
                return Color.green;
            case 4:
                return Color.yellow;
            case 5:
                return Color.magenta;
            case 6:
                return Color.cyan;
            default:
                return Color.white;
        }
    }

    private PlayerShip GetShip(int id) {
        foreach (PlayerShip ship in ships) {
            if (ship.Id == id) {
                return ship;
            }
        }
        return null;
    }

    void Update() {
        if (IsGameStarted) {
            bool ended = true;
            foreach (PlayerShip ship in ships) {
                if (ship.life > 0) {
                    ended = false;
                }
            }
            if (ended) {
                IsGameEnded = true;
                GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject asteroid in asteroids) {
                    Destroy(asteroid);
                }
            }
        }
    }

    void OnGUI() {
        if (!IsGameStarted) {
            if (Network.peerType == NetworkPeerType.Disconnected) {
                GUILayout.Label("Game server Offline");
                if (GUILayout.Button("Start Game Server")) {
                    StartServer();
                }
            } else {
                if (ships.Count < 1) {
                    GUILayout.Label("Waiting for players to connect");
                } else if (!IsGameStarted) {
                    GUILayout.Label("Waiting for a player to start game");
                }
            }
        }
        if (IsGameEnded) {
            GUILayout.Label("Game ended.");
            foreach (PlayerShip ship in ships) {
                GUILayout.Label("Player " + ship.Id + " - " + ship.Score);
            }
        }
    }
}
