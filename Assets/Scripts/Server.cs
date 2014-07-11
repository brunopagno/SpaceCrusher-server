using System.Collections.Generic;
using System.IO;
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

    private float meteorRushTime = 0;
    private bool meteorRush = false;
    private List<GameObject> rushedMeteors = new List<GameObject>();

    private string gameIdentifier = "";
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

    #region RPCOut
    [RPC]
    public void RPCOut(string info) {
        networkView.RPC("RPCIn", RPCMode.Others, info);
    }

    [RPC]
    public void SetBulletsGun1(string message) {
        networkView.RPC("SetBulletsGun1", RPCMode.Others, message);
    }

    [RPC]
    public void SetBulletsGun2(string message) {
        networkView.RPC("SetBulletsGun2", RPCMode.Others, message);
    }

    [RPC]
    void SetGun2WithSound(string message) {
        networkView.RPC("SetBulletsGun2", RPCMode.Others, message);
    }

    [RPC]
    public void SetBulletsGun3(string message) {
        networkView.RPC("SetBulletsGun3", RPCMode.Others, message);
    }

    [RPC]
    void SetGun3WithSound(string message) {
        networkView.RPC("SetBulletsGun3", RPCMode.Others, message);
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
        networkView.RPC("SetLife", RPCMode.Others, message);
    }
    #endregion

    #region RPCIn
    [RPC]
    void PassarArminhaProAmiguinho(string message) {
        string[] d = message.Split(':');
        PlayerShip s = GetShip(int.Parse(d[0]));
        PlayerShip ss = GetOtherShip(s.Id);
        if (d[1].Equals("gun2")) {
            s.gun2Ammo--;
            ss.gun2Ammo++;
            SetBulletsGun2(s.Id + ":" + s.gun2Ammo);
            SetGun2WithSound(ss.Id + ":" + ss.gun2Ammo);
            s.gun2_sent++;
        } else {
            s.gun3Ammo--;
            ss.gun3Ammo++;
            SetBulletsGun3(s.Id + ":" + s.gun3Ammo);
            SetGun3WithSound(ss.Id + ":" + ss.gun3Ammo);
            s.gun3_sent++;
        }
    }

    [RPC]
    void PassarVidaProAmiguinho(string message) {
        string[] d = message.Split(':');
        PlayerShip s = GetShip(int.Parse(d[0]));
        PlayerShip ss = GetOtherShip(s.Id);
        if (s.life > 1) {
            s.life--;
            ss.life++;
            SetLife(s.Id + ":" + s.life);
            SetLifeWithSound(ss.Id + ":" + ss.life);
            s.life_sent++;
        }
    }

    [RPC]
    void ChangeGun(string message) {
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
        StartGame();
    }

    [RPC]
    void RouletResult(string message) {
        string[] d = message.Split(':');
        PlayerShip s = GetShip(int.Parse(d[0]));

        int result = int.Parse(d[1]);
        switch (result) {
            case 1:
                s.RouletteResult(0, 0, 1);
                break;
            case 2:
                s.RouletteResult(1, 0, 0);
                break;
            case 3:
                s.RouletteResult(0, 1, 0);
                break;
            case 4:
                StartRush();
                break;
        }
    }

    [RPC]
    public void SendPosition(string position) { }

    [RPC]
    public void SendChangedGun(string gun) { }

    [RPC]
    public void SendGun(string _gun) { }

    [RPC]
    public void SendLife() { }
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

            if (meteorRush) {
                meteorRushTime += Time.deltaTime;
                if (meteorRushTime > 5) {
                    meteorRush = false;
                    meteorRushTime = 0;
                    EndRush();
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

        // FOR DEBUG ONLY
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    if (meteorRush) {
        //        EndRush();
        //    } else {
        //        StartRush();
        //    }
        //}
    }

    private void StartRush() {
        rushes++;
        meteorRush = true;
        for (int i = 0; i < 4; i++) {
            rushedMeteors.Add((GameObject)Instantiate(asteroideePrefab, new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0), Quaternion.identity));
        }
    }

    private void EndRush() {
        foreach (GameObject rushy in rushedMeteors) {
            Destroy(rushy);
        }
        meteorRush = false;
    }

    private void StartGame() {
        state = GameState.Started;
        networkView.RPC("RPCStart", RPCMode.Others, string.Empty);

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
        SetLog();
        state = GameState.Ended;
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject asteroid in asteroids) {
            Destroy(asteroid);
        }
        foreach (PlayerShip ship in ships) {
            ship.EndedGame();
        }
    }

    private void SetLog() {
        string fileName = "results_" + gameIdentifier + "_difficulty_" + difficulty + ".txt";
        if (File.Exists(fileName)) {
            fileName = "results_desambiguation_" + gameIdentifier + ".txt";
        }
        StreamWriter writer = File.CreateText(fileName);
        writer.WriteLine("id,score,remaining_life,remaining_ammo_2,remaining_ammo_3,remaining_special,times_gun2,times_gun3,times_special," +
                         "times_hit,life_collected,special_collected,roulette_rounds,gun2_sent,gun3_sent,life_sent");
        PlayerShip s = GetShip(1);
        PlayerShip ss = GetOtherShip(1);
        writer.Write(s.Id + "," +
                     s.Score + "," +
                     s.life + "," +
                     s.gun2Ammo + "," +
                     s.gun3Ammo + "," +
                     s.specialAmmo + "," +
                     s.timesGun2 + "," +
                     s.timesGun3 + "," +
                     s.timesSpecial + "," +
                     s.timesHit + "," +
                     s.lifeCollected + "," +
                     s.specialCollected + "," +
                     s.rouletteRounds + "," +
                     s.gun2_sent + "," +
                     s.gun3_sent + "," +
                     s.life_sent);
        writer.Write(ss.Id + "," +
                     ss.Score + "," +
                     ss.life + "," +
                     ss.gun2Ammo + "," +
                     ss.gun3Ammo + "," +
                     ss.specialAmmo + "," +
                     ss.timesGun2 + "," +
                     ss.timesGun3 + "," +
                     ss.timesSpecial + "," +
                     ss.timesHit + "," +
                     ss.lifeCollected + "," +
                     ss.specialCollected + "," +
                     ss.rouletteRounds + "," +
                     ss.gun2_sent + "," +
                     ss.gun3_sent + "," +
                     ss.life_sent);

        writer.WriteLine("---");
        writer.WriteLine("TotalRemainingTime:" + gameTime);
        writer.WriteLine("Rushes:" + rushes);

        writer.Flush();
        writer.Close();
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
            if (meteorRush) {
                GUILayout.Label("IN RUSH");
            }
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
