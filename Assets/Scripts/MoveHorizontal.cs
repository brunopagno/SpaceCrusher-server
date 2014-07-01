using UnityEngine;
using System.Collections;

public class MoveHorizontal : MonoBehaviour {

  public float speed = 0.1f;
  protected Vector3 movement;

	void Start () {
        this.movement = new Vector3();
	}
	
	void Update () {
      if (Input.GetKeyDown(KeyCode.LeftArrow)) {
          this.transform.position += Vector3.left * speed;
      }
      if (Input.GetKeyDown(KeyCode.RightArrow)) {
		  this.transform.position += Vector3.right * speed;
      }
	}
}
