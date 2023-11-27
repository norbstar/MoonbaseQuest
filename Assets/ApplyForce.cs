using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour
{
    [SerializeField] InputAction action;

    private Rigidbody rigidbody;
        
    void Awake() => rigidbody = GetComponent<Rigidbody>();

    void OnEnable() => action.Enable();

    void OnDisable() => action.Disable();

    // Update is called once per frame
    // void Update()
    // {
    //     if (action.triggered)
    //     {
    //         Debug.Log($"AddForce");
    //         rigidbody.AddForce(-Physics.gravity * 2f);
    //     }
    // }

    void FixedUpdate()
    {
        // rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // rigidbody.AddForce(-Physics.gravity * 2f);
        // rigidbody.constraints = RigidbodyConstraints.None;

        // Debug.Log($"Force: {rigidbody.velocity}");
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject trigger = collision.gameObject;

        if (trigger.name.Equals("Force Plane"))
        {
            rigidbody.AddForce(-Physics.gravity * 80f);
        }
    }
}
