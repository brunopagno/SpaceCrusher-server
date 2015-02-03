using UnityEngine;
using System.Collections;

public class BombBehaviour : MonoBehaviour {

    private PlayerShip owner;
    private float bombSpeed = 5f;
    public ParticleSystem explosion;

	public void Launch(PlayerShip owner) {
        this.owner = owner;
    }

    void Update() {
        transform.Translate(Vector3.down * Time.deltaTime * bombSpeed);
        if (Mathf.Abs(transform.position.y - owner.transform.position.y) <= 0.4f) {
            RemoveBomb();
        }
    }

    public void RemoveBomb() {
        if (transform.position.y > -1) {
            // TODO
            //audio.clip = explosionSound
            //audio.Play ();
        }
        Instantiate(explosion, transform.position, transform.rotation);
        owner.AfterBombDropped();
        Destroy(gameObject);
    }

}

