using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

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

    public float speed = 8f;

    void Update() {
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(horizontal, 0, 0);
    }

    internal void RemoveFromGame() {
        Destroy(gameObject);
    }
}
