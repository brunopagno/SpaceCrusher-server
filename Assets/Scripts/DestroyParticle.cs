using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {
	void Update () {
		if (!particleSystem.IsAlive()) {
			Destroy (gameObject);
		}
	}
}
