using UnityEngine;
using System.Collections;

public class CollectableBehaviour : MonoBehaviour {
    public float speed = 1;
    public int something;

    void Start() { }

    void Update() {
        transform.Translate(Vector3.down * Time.deltaTime * speed);
        if (transform.position.y <= -12) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (something == 1) {
                other.gameObject.GetComponent<PlayerShip>().CollectLife();
            } else if (something == 2) {
                other.gameObject.GetComponent<PlayerShip>().CollectSpecial();
            } else {
                // TODO
                //other.gameObject.GetComponent<PlayerShip>().CollectCoin();
            }
            Destroy(gameObject);
        }
    }
}
