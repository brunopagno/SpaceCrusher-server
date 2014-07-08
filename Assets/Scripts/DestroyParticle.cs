using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
		if (!particleSystem.IsAlive()) {
			Destroy (gameObject);
		}
	}
}
