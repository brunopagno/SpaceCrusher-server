using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

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

    private Color color;
    public SpriteRenderer dotRenderer;

    public Collider2D shipCollider;

    public Transform bulletSocket;
    public GameObject bullet;
    public GameObject bulletOther;

    public int life = 3;
    private float time = 0;
    private float gunTime = 0;

    private int gun2Ammo = 1;
    private int gun3Ammo = 1;
    private int specialAmmo = 0;
    private int gun = 1;
    
    private bool endedGame;

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
        if (endedGame) {
            return;
        }
        time += Time.deltaTime;
        if (life == 0) {
            if (time > 3) {
                Destroy(gameObject.GetComponent<Rigidbody2D>());
                Destroy(gameObject.GetComponent<CircleCollider2D>());
                Destroy(gameObject.GetComponent<SpriteRenderer>());
            }
            return;
        }
        switch (gun) {
            case 1:
                if (time > 0.46f) {
                    time = 0;
                    GameObject bb = (GameObject)Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
                    bb.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                break;
            case 2:
                if (time > 0.05f) {
                    time = 0;
                    GameObject bb = (GameObject)Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
                    bb.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                gunTime += Time.deltaTime;
                if (gunTime > 2f) {
                    this.SetGun("gun1");
                    gunTime = 0;
                }
                break;
            case 3:
                if (time > 0.5f) {
                    time = 0;
                    GameObject bba = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position, bullet.transform.rotation);
                    GameObject bbb = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position + Vector3.right, bullet.transform.rotation);
                    GameObject bbc = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position + Vector3.left, bullet.transform.rotation);
                    bba.GetComponent<BulletBehaviour>().AssignOwner(this);
                    bbb.GetComponent<BulletBehaviour>().AssignOwner(this);
                    bbc.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                gunTime += Time.deltaTime;
                if (gunTime > 2f) {
                    this.SetGun("gun1");
                    gunTime = 0;
                }
                break;
            case 4:
                break;
        }
    }

    public void HitScore() {
        GameObject server = GameObject.Find("Server");
        this.score++;
        server.GetComponent<Server>().SyncScore(Id + ":" + score.ToString());
    }

    public void TakeHit() {
        if (life == 0) {
            return;
        }
        GameObject server = GameObject.Find("Server");
        this.life--;
        server.GetComponent<Server>().SetLife(Id + ":" + life.ToString());
    }

    public void SetGun(string gun) {
        if (gun.Equals("gun1")) {
            this.gun = 1;
        } else if (gun.Equals("gun2")) {
            gun2Ammo--;
            if (gun2Ammo < 0) {
                gun2Ammo = 0;
                return;
            }
            this.gun = 2;
            GameObject server = GameObject.Find("Server");
            server.GetComponent<Server>().SetBulletsGun2("" + Id + ":" + gun2Ammo);
        } else if (gun.Equals("gun3")) {
            gun3Ammo--;
            if (gun3Ammo < 0) {
                gun3Ammo = 0;
                return;
            }
            this.gun = 3;
            GameObject server = GameObject.Find("Server");
            server.GetComponent<Server>().SetBulletsGun3("" + Id + ":" + gun3Ammo);
        }

        if (gun.Equals("gunSpecial")) {
            specialAmmo--;
            if (specialAmmo < 0) {
                specialAmmo = 0;
                return;
            }
            this.gun = 4;
            GameObject server = GameObject.Find("Server");
            specialAmmo--;
            server.GetComponent<Server>().SetBulletsSpecial("" + Id + ":" + specialAmmo);
            shipCollider.enabled = false;
            this.color = this.dotRenderer.color;
            this.dotRenderer.color = new Color(this.color.r, this.color.g, this.color.b, 0.2f);
        } else {
            this.dotRenderer.color = this.color;
            shipCollider.enabled = true;
        }
    }

    public void CollectLife() {
        GameObject server = GameObject.Find("Server");
        life++;
        server.GetComponent<Server>().SetLife("" + Id + ":" + life);
    }

    public void CollectSpecial() {
        GameObject server = GameObject.Find("Server");
        specialAmmo++;
        server.GetComponent<Server>().SetBulletsSpecial("" + Id + ":" + specialAmmo);
    }

    public void RouletteResult(int ammo2, int ammo3, int lifep) {
        GameObject server = GameObject.Find("Server");
        gun2Ammo += ammo2;
        gun3Ammo += ammo3;
        life += lifep;
        server.GetComponent<Server>().SetBulletsGun2("" + Id + ":" + gun2Ammo);
        server.GetComponent<Server>().SetBulletsGun3("" + Id + ":" + gun3Ammo);
        server.GetComponent<Server>().SetLife("" + Id + ":" + life);
    }

    public void EndedGame() {
        endedGame = true;
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        Destroy(gameObject.GetComponent<SpriteRenderer>());
    }
}
