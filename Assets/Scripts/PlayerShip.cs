using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    public SpriteRenderer dotRenderer;

    //public AudioClip[] shotSound;
    public Transform bulletSocket;
    public GameObject bullet;

    public int life = 3;
    private float time = 0;

    private int score = 0;
    public int Score {
        get { return this.score; }
    }

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
        if (life == 0) {
            return;
        }
        Vector3 cv = Camera.main.ScreenToWorldPoint(new Vector3((x * Screen.width), transform.position.y, transform.position.z));
        transform.position = new Vector3(cv.x, transform.position.y, transform.position.z);
    }

    void Update() {
        time += Time.deltaTime;
        if (life == 0) {
            if (time > 3) {
                Destroy(gameObject.GetComponent<Rigidbody2D>());
                Destroy(gameObject.GetComponent<CircleCollider2D>());
                Destroy(gameObject.GetComponent<SpriteRenderer>());
            }
            return;
        }
        if (time > 0.33f) {
            time = 0;
            GameObject bb = (GameObject)Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
            bb.GetComponent<BulletBehaviour>().owner = this;
            //audio.clip = shotSound[Random.Range(0, 3)];
            //audio.Play();
        }
    }

    public void HitScore() {
        this.score++;
    }

    public void TakeHit() {
        if (life == 0) {
            return;
        }
        GameObject server = GameObject.Find("Server");
        this.life--;
        server.GetComponent<Server>().SetLife(Id + ":" + life.ToString());
    }
}
