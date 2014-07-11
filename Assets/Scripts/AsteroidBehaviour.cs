using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

    public float asteroidSpeed = 1;
    public ParticleSystem explosion;
    public bool dieOnCollide = false;

    //public AudioClip[] explosionSound;

    void Start() { }

    void Update() {
        transform.Translate(Vector3.down * Time.deltaTime * asteroidSpeed);
        if (transform.position.y <= -12) {
            ResetEnemyPosition();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Instantiate(explosion, transform.position, transform.rotation);
            other.gameObject.GetComponent<PlayerShip>().TakeHit();
            ResetEnemyPosition();
        }
        if (other.tag == "Bullet") {
            other.gameObject.GetComponent<BulletBehaviour>().Owner.HitScore();
            Instantiate(explosion, other.gameObject.transform.position, other.gameObject.transform.rotation);
            Destroy(other.gameObject);
            ResetEnemyPosition();
        }
    }

    public void ResetEnemyPosition() {
        if (dieOnCollide) {
            Destroy(gameObject);
        }
        if (transform.position.y > -1) {
            //audio.clip = explosionSound [Random.Range (0, 2)];
            //audio.Play ();
        }
        transform.position = new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0);
    }
}
