using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Server : MonoBehaviour {

    private enum GameState {
        Unstarted, Started, Ended, Special
    }

    public int port = 32154;
    public int maxConnections = 4;
    public PlayerShip shipPrefab;
    public GameObject asteroidPrefab;
    public GameObject asteroideePrefab;
    public GameObject specialPrefab;
    public GameObject lifePrefab;

    private int PLAYER_ID = 1;
    private List<PlayerShip> ships = new List<PlayerShip>();
    private GameState state;
    private bool theAsteroidsAreThere = false;

    public string difficulty = "1";

    private const string TYPE_NAME = "IHA-SPG0";
    private const string GAME_NAME = "SpaceCrusher Game";

    private float gameTime = 10;
    private float extraTimer = 12.8f;

    private string gameIdentifier = "1";
    private int rushes;

    void StartServer() {
        Network.InitializeServer(maxConnections, port, false);
        MasterServer.RegisterHost(TYPE_NAME, GAME_NAME);
        state = GameState.Unstarted;
    }

    void StopServer() {
        Network.Disconnect();
        foreach (PlayerShip ship in ships) {
            ship.RemoveFromGame();
        }
        ships.Clear();
    }

    void OnPlayerConnected(NetworkPlayer player) {
        if (state == GameState.Unstarted) {
            PlayerShip ship = (PlayerShip)Instantiate(shipPrefab);
            ship.dotRenderer.color = GetPlayerColor(PLAYER_ID);
            ship.Id = NextPlayerId();
            ship.Player = player;
            ships.Add(ship);
            RPCConnect("PID:" + ship.Id);
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

    #region RPCOut

    [RPC]
    void RPCConnect(string message) {
        networkView.RPC("RPCConnect", RPCMode.Others, message);
    }

    [RPC]
    public void SetBulletsGun2(string message) {
        networkView.RPC("SetBulletsGun2", RPCMode.Others, message);
    }

    [RPC]
    void SetGun2WithSound(string message) {
        networkView.RPC("SetGun2WithSound", RPCMode.Others, message);
    }

    [RPC]
    public void SetBulletsGun3(string message) {
        networkView.RPC("SetBulletsGun3", RPCMode.Others, message);
    }

    [RPC]
    void SetGun3WithSound(string message) {
        networkView.RPC("SetGun3WithSound", RPCMode.Others, message);
    }

    [RPC]
    public void SetBulletsSpecial(string message) {
        networkView.RPC("SetBulletsSpecial", RPCMode.Others, message);
    }

    [RPC]
    public void SyncScore(string message) {
        networkView.RPC("SyncScore", RPCMode.Others, message);
    }

    [RPC]
    public void SetLife(string message) {
        networkView.RPC("SetLife", RPCMode.Others, message);
    }

    [RPC]
    void SetLifeWithSound(string message) {
        networkView.RPC("SetLifeWithSound", RPCMode.Others, message);
    }

    #endregion

    #region RPCIn

    [RPC]
    void SyncPosition(string message) {
        Match m = Regex.Match(message, "\\d*:\\d*");
        if (m.Success) {
            string[] d = message.Split(':');
            PlayerShip s = GetShip(int.Parse(d[0]));
            s.MoveTo(float.Parse(d[1]));
        }
    }

    [RPC]
    void SyncChangedItem(string message) {
        // TODO: Adapt for gun, lightning and life
        string[] d = message.Split(':');
        PlayerShip s = GetShip(int.Parse(d[0]));
        if (d[1].Equals("gunSpecial")) {
            if (s.specialAmmo > 0) {
                state = GameState.Special;
                s.SetGun(d[1]);
            }
        } else {
            state = GameState.Started;
            s.SetGun(d[1]);
        }
    }

    [RPC]
    void RPCStart(string nothing) {
        StartGame();
    }

    #endregion

    private int NextPlayerId() {
        return PLAYER_ID++;
    }

    private Color GetPlayerColor(int id) {
        switch (id) {
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

    private PlayerShip GetOtherShip(int id) {
        foreach (PlayerShip ship in ships) {
            if (ship.Id != id) {
                return ship;
            }
        }
        return null;
    }

    void Update() {
        if (state == GameState.Started || state == GameState.Special) {
            extraTimer -= Time.deltaTime;
            if (extraTimer <= 0) {
                extraTimer = Random.Range(8f, 15f);
                int rand = Random.Range(0, 99);
                if (rand % 2 == 0) {
                    Instantiate(specialPrefab, new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0), Quaternion.identity);
                } else {
                    Instantiate(lifePrefab, new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0), Quaternion.identity);
                }
            }

            bool ended = true;
            foreach (PlayerShip ship in ships) {
                if (ship.life > 0) {
                    ended = false;
                }
            }
            gameTime -= Time.deltaTime;
            if (gameTime < 0) {
                gameTime = 0;
                ended = true;
            }
            if (ended) {
                EndGame();
            }
        }
    }

    private void StartGame() {
        state = GameState.Started;

        if (!theAsteroidsAreThere) {
            theAsteroidsAreThere = true;
            int amount = 1;
            if (!int.TryParse(difficulty, out amount)) {
                amount = 1;
            }
            amount *= 5;
            for (int i = 0; i < amount; i++) {
                Instantiate(asteroidPrefab, new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0), Quaternion.identity);
            }
        }

        foreach (PlayerShip ship in ships) {
            SetLife(ship.Id + ":" + ship.life);
        }
    }

    private void EndGame() {
        state = GameState.Ended;
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject asteroid in asteroids) {
            Destroy(asteroid);
        }
        foreach (PlayerShip ship in ships) {
            ship.EndedGame();
        }
    }

    void OnGUI() {
        if (state == GameState.Unstarted) {
            if (Network.peerType == NetworkPeerType.Disconnected) {
                GUILayout.Label("GameIdentifier:");
                gameIdentifier = GUILayout.TextField(gameIdentifier);
                GUILayout.Label("Game server Offline");
                if (GUILayout.Button("Start Game Server")) {
                    StartServer();
                }
                GUILayout.Label("Difficulty (int):");
                difficulty = GUILayout.TextField(difficulty);
            } else {
                if (ships.Count < 1) {
                    GUILayout.Label("Waiting for players to connect");
                } else {
                    GUILayout.Label("Waiting for a player to start game");
                }
            }
        }
        if (state == GameState.Started || state == GameState.Special) {
            GUILayout.Label("Seconds remaining: " + gameTime.ToString("N"));
        }
        if (state == GameState.Ended) {
            GUILayout.Label("Game [" + gameIdentifier + "] ended.");
            foreach (PlayerShip ship in ships) {
                GUI.contentColor = GetPlayerColor(ship.Id);
                GUILayout.Label("Player " + ship.Id + ": " + ship.Score + " points");
            }
        }
    }
}
