using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    enum ShipState {
        Normal, Shaken
    }

    protected int id;
    public int Id {
        get { return this.id; }
        set { this.id = value; }
    }

    private int score = 0;
    public int Score {
        get { return this.score; }
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

    private ShipState state = ShipState.Normal;

    public int life = 3;
    private float timer = 0;
    private float gunTimer = 0;
    private float bombTimer = -10;

    public int gun2Ammo = 2;
    public int gun3Ammo = 2;
    public int specialAmmo = 0;
    private int gun = 1;

    public float gunSpeed = 0.46f;

    private bool endedGame;

    public int timesGun2;
    public int timesGun3;
    public int timesSpecial;
    public int timesHit;
    public int lifeCollected;
    public int specialCollected;
    public int gun2_sent;
    public int gun3_sent;
    public int life_sent;

    void Start() {
        this.color = dotRenderer.color;
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
        if (endedGame) {
            return;
        }
        timer += Time.deltaTime;
        if (life == 0) {
            if (timer > 3) {
                Destroy(gameObject.GetComponent<Rigidbody2D>());
                Destroy(gameObject.GetComponent<BoxCollider2D>());
                Destroy(gameObject.GetComponent<SpriteRenderer>());
            }
            return;
        }
        switch (gun) {
            case 1:
                if (timer > gunSpeed) {
                    timer = 0;
                    GameObject bb = (GameObject)Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
                    bb.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                break;
            case 2:
                if (timer > 0.05f) {
                    timer = 0;
                    GameObject bb = (GameObject)Instantiate(bullet, bulletSocket.transform.position, bullet.transform.rotation);
                    bb.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                gunTimer += Time.deltaTime;
                if (gunTimer > 2f) {
                    this.SetGun("gun1");
                    gunTimer = 0;
                }
                break;
            case 3:
                if (timer > 0.5f) {
                    timer = 0;
                    GameObject bba = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position, bullet.transform.rotation);
                    GameObject bbb = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position + Vector3.right, bullet.transform.rotation);
                    GameObject bbc = (GameObject)Instantiate(bulletOther, bulletSocket.transform.position + Vector3.left, bullet.transform.rotation);
                    bba.GetComponent<BulletBehaviour>().AssignOwner(this);
                    bbb.GetComponent<BulletBehaviour>().AssignOwner(this);
                    bbc.GetComponent<BulletBehaviour>().AssignOwner(this);
                }
                gunTimer += Time.deltaTime;
                if (gunTimer > 2f) {
                    this.SetGun("gun1");
                    gunTimer = 0;
                }
                break;
            case 4:
                break;
        }
        if (bombTimer > -5f) {
            bombTimer -= Time.deltaTime;
            if (bombTimer < 0) {
                bombTimer = -10;
                RecoverAfterBomb();
            }
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
        timesHit++;
        GameObject server = GameObject.Find("Server");
        this.life--;
        server.GetComponent<Server>().SetLife(Id + ":" + life.ToString());
    }
    
    public void HitByBomb() {
        bombTimer = 5;
        gunSpeed *= 2;
        // TODO add something that shows that the player is slow
        // TODO send move speed reduction message
    }

    public void RecoverAfterBomb() {
        gunSpeed /= 2;
        // TODO remove something that shows that the player is slow
        // TODO send move speed recovery message
    }

    public void SetGun(string gun) {
        if (gun.Equals("gun1")) {
            this.gun = 1;
        } else if (gun.Equals("gun2")) {
            timesGun2++;
            gun2Ammo--;
            if (gun2Ammo < 0) {
                gun2Ammo = 0;
                return;
            }
            this.gun = 2;
            GameObject server = GameObject.Find("Server");
            server.GetComponent<Server>().SetBulletsGun2("" + Id + ":" + gun2Ammo);
        } else if (gun.Equals("gun3")) {
            timesGun3++;
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
            timesSpecial++;
            specialAmmo--;
            if (specialAmmo < 0) {
                specialAmmo = 0;
                return;
            }
            this.gun = 4;
            GameObject server = GameObject.Find("Server");
            server.GetComponent<Server>().SetBulletsSpecial("" + Id + ":" + specialAmmo);
            shipCollider.enabled = false;
            this.color = this.dotRenderer.color;
            this.dotRenderer.color = new Color(this.color.r, this.color.g, this.color.b, 0.2f);
        } else {
            this.dotRenderer.color = this.color;
            shipCollider.enabled = true;
        }
        timer = 0;
    }

    public void CollectLife() {
        lifeCollected++;
        GameObject server = GameObject.Find("Server");
        life++;
        server.GetComponent<Server>().SetLife("" + Id + ":" + life);
    }

    public void CollectSpecial() {
        specialCollected++;
        GameObject server = GameObject.Find("Server");
        specialAmmo++;
        server.GetComponent<Server>().SetBulletsSpecial("" + Id + ":" + specialAmmo);
    }

    public void EndedGame() {
        endedGame = true;
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(gameObject.GetComponent<SpriteRenderer>());
    }
}
