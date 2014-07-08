using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

	public float asteroidSpeed;
	public ParticleSystem explosion;

	public int points;

	public AudioClip[] explosionSound;

	void Start () {
	
	}

	void Update () {
		transform.Translate(Vector3.down * Time.deltaTime * asteroidSpeed);
		if (transform.position.y <= -4) {
			ResetEnemyPosition ();
		}
	}

	void OnTriggerEnter (Collider other){
		if (other.collider.tag == "Player") {
			Instantiate (explosion, transform.position, transform.rotation);
            //GameController.gameLives -= 1;
			ResetEnemyPosition();
		}
	}

	public void ResetEnemyPosition () {
		if (transform.position.y > -1) {
			audio.clip = explosionSound [Random.Range (0, 2)];
			audio.Play ();
		}
		transform.position = new Vector3 (Random.Range(-6, 7), 10, 0);
	}
}
