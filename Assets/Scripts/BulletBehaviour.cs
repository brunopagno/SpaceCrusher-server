using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

    public float bulletSpeed = 8;
    public ParticleSystem explosion;

    //public AudioClip[] explosionSound;

    void Start() { }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy") {
            Instantiate(explosion, transform.position, transform.rotation);
            //GameController.addScore(other.gameObject.GetComponent<AsteroidBehaviour>().points);
            Destroy(gameObject);
        }
    }

    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * bulletSpeed);
        if (transform.position.y >= 8) {
            Destroy(gameObject);
        }
    }
}
