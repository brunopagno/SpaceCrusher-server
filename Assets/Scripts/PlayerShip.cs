using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    public SpriteRenderer dotRenderer;
    public GameObject ship;

    protected int id;
    public int Id {
        get { return this.id; }
        set { this.id = value; }
    }

    protected NetworkPlayer player;
    public NetworkPlayer Player {
        get { return this.player; }
        set { this.player = value; }
    }

    internal void RemoveFromGame() {
        Destroy(gameObject);
    }
}
