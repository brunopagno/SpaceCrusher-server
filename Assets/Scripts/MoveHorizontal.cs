using UnityEngine;
using System.Collections;

public class MoveHorizontal : MonoBehaviour {

    public float speed = 2.5f;
    
    void Update() {
        float transV = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float transH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(transH, transV, 0);
    }
}
