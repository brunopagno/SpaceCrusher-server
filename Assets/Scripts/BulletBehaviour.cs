using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

	public float bulletSpeed = 40;
	public ParticleSystem explosion;
	
	public AudioClip[] explosionSound;

	void Start () {
	
	}

	public void OnTriggerEnter (Collider other){
		if (other.collider.tag == "Enemy") {
			Instantiate (explosion, transform.position, transform.rotation);
            //GameController.addScore(other.gameObject.GetComponent<AsteroidBehaviour>().points);
			other.gameObject.GetComponent<AsteroidBehaviour>().ResetEnemyPosition();
			Destroy (gameObject);
		}
	}
}
