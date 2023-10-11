using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ArrowTest : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform tip;

    [Header("Config")]
    [SerializeField] float speed = 2000f;

    private new Rigidbody rigidbody;
    private bool inMotion;
    private Vector3 lastPosition;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start() => Fire(1f);

    void FixedUpdate()
    {
        if (!inMotion) return;

        // rigidbody.MoveRotation(Quaternion.LookRotation(rb.velocity, transform.up));

        if (Physics.Linecast(lastPosition, tip.position))
        {
            Stop();
        }

        lastPosition = tip.position;
    }

    private void Stop()
    {
        // Debug.Log("Stop");
        inMotion = false;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    public void Fire(float pullValue)
    {
        // Debug.Log("Fire");
        inMotion = true;

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.AddForce(transform.forward * (pullValue * speed));

        // Destroy(gameObject, 5f);
    }
}
