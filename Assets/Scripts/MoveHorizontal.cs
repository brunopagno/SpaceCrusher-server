using UnityEngine;
using System.Collections;

public class MoveHorizontal : MonoBehaviour {

    public float speed = 2.5f;
    
    void Update() {
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(horizontal, 0, 0);
    }
}
