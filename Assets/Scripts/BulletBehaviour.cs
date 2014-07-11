using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

    public float bulletSpeed = 8;
    public ParticleSystem explosion;
    public SpriteRenderer theRenderer;
    private PlayerShip owner;
    public PlayerShip Owner {
        get { return owner; }
    }

    public void AssignOwner(PlayerShip ow) {
        this.owner = ow;
        switch (ow.Id) {
            case 1:
                this.theRenderer.color = Color.blue; break;
            case 2:
                this.theRenderer.color = Color.red; break;
            case 3:
                this.theRenderer.color = Color.green; break;
            case 4:
                this.theRenderer.color = Color.yellow; break;
            case 5:
                this.theRenderer.color = Color.magenta; break;
            case 6:
                this.theRenderer.color = Color.cyan; break;
        }
    }

    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * bulletSpeed);
        if (transform.position.y >= 8) {
            Destroy(gameObject);
        }
    }
}
