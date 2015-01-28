using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

    private float asteroidSpeed;
    public ParticleSystem explosion;

    void Start() {
        this.asteroidSpeed = Random.Range(3.5f, 6.5f);
    }

    void Update() {
        transform.Translate(Vector3.down * Time.deltaTime * asteroidSpeed);
        if (transform.position.y <= -12) {
            RemoveAsteroid();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Instantiate(explosion, transform.position, transform.rotation);
            other.gameObject.GetComponent<PlayerShip>().TakeHit();
            RemoveAsteroid();
        }
        if (other.tag == "Bullet") {
            other.gameObject.GetComponent<BulletBehaviour>().Owner.HitScore();
            Instantiate(explosion, other.gameObject.transform.position, other.gameObject.transform.rotation);
            Destroy(other.gameObject);
            RemoveAsteroid();
        }
    }

    public void RemoveAsteroid() {
        if (transform.position.y > -1) {
            // TODO
            //audio.clip = explosionSound
            //audio.Play ();
        }
        Server server = GameObject.Find("Server").GetComponent<Server>();
        server.CreateAsteroid();
        Destroy(gameObject);
    }
}
