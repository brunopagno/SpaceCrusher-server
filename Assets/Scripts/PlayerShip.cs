using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    public SpriteRenderer dotRenderer;

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

    public void MoveTo(float x) {
        Vector3 cv = Camera.main.ScreenToWorldPoint(new Vector3((x * Screen.width), transform.position.y, transform.position.z));
        transform.position = new Vector3(cv.x, transform.position.y, transform.position.z);
    }
}
