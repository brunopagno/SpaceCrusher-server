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
        if ((transform.position.y < owner.transform.position.y - 0.4f) || (transform.position.y < -12)) {
            RemoveBomb();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Debug.Log("player was hit by bomb");
            Instantiate(explosion, transform.position, transform.rotation);
            other.gameObject.GetComponent<PlayerShip>().TakeHit();
            other.gameObject.GetComponent<PlayerShip>().HitByBomb();
            RemoveBomb();
        }
    }

    public void RemoveBomb() {
        owner.AfterBombDropped();
        Destroy(gameObject);
    }

}

