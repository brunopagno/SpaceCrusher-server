using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    public SpriteRenderer dotRenderer;

    //public AudioClip[] shotSound;
    public Transform bulletSocket;
    public GameObject bullet;

    private float time = 0;

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

    void Update() {
        time += Time.deltaTime;
        if (time > 0.2f) {
            time = 0;
            Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
            //audio.clip = shotSound[Random.Range(0, 3)];
            //audio.Play();
        }
    }
}
