using UnityEngine;

public class VelocityArrow : MonoBehaviour
{
    // public float speed = 5f;
    public float force = 200f;

    private new Rigidbody rigidbody;

    void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    // void Update() => this.rigidbody.velocity = Vector3.forward * speed;

    void Update() => this.rigidbody.AddForce(Vector3.forward * force);
}
