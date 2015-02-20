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

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Instantiate(explosion, transform.position, transform.rotation);
            other.gameObject.GetComponent<PlayerShip>().HitByBomb();
            RemoveBomb();
        }
    }

    public void RemoveBomb() {
        Instantiate(explosion, transform.position, transform.rotation);
        owner.AfterBombDropped();
        Destroy(gameObject);
    }

}

