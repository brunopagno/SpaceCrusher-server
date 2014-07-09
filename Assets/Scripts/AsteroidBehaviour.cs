using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

    public float asteroidSpeed = 1;
    public ParticleSystem explosion;

    //public AudioClip[] explosionSound;

    void Start() { }

    void Update() {
        transform.Translate(Vector3.down * Time.deltaTime * asteroidSpeed);
        if (transform.position.y <= -4) {
            ResetEnemyPosition();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Instantiate(explosion, transform.position, transform.rotation);
            //GameController.gameLives -= 1;
            ResetEnemyPosition();
        }
        if (other.tag == "Bullet") {
            ResetEnemyPosition();
        }
    }

    public void ResetEnemyPosition() {
        if (transform.position.y > -1) {
            //audio.clip = explosionSound [Random.Range (0, 2)];
            //audio.Play ();
        }
        transform.position = new Vector3(Random.Range(-7, 7), Random.Range(9, 13), 0);
    }
}
