using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

    public float bulletSpeed = 8;
    public ParticleSystem explosion;
    public PlayerShip owner;

    //public AudioClip[] explosionSound;

    void Start() { }

    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * bulletSpeed);
        if (transform.position.y >= 8) {
            Destroy(gameObject);
        }
    }
}
