using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class ProtoTest : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform body;

    [Header("Config")]
    [SerializeField] AnimationCurve curve;

    [Header("Velocity")]
    [Range(10f, 200f)]
    [SerializeField] float forceMultiplier = 100f;
    [Range(0f, 20f)]
    [SerializeField] float maxVelocity = 10f;
    [Range(5f, 20f)]
    [SerializeField] float maxVelocityAngle = 15f;

    [Header("Bindings")]
    [SerializeField] InputAction action;

    [Header("Stats")]
    [SerializeField] float headingVelocity = 0f;
    [SerializeField] float attackAngle = 0f;

    private Rigidbody rigidbody;
    private float startSec, elapsedTimeSec;
    private bool isPressed;

    void Awake() => rigidbody = GetComponent<Rigidbody>();

    void OnEnable() => action.Enable();

    void OnDisable() => action.Disable();

    // Update is called once per frame
    void Update()
    {
        if (action.triggered)
        {
            startSec = Time.time;
            elapsedTimeSec = 0f;
        }

        isPressed = action.IsPressed();
    }

    void FixedUpdate()
    {
        if (isPressed)
        {
            var value = action.ReadValue<float>();
            elapsedTimeSec += Time.deltaTime;
            var normal = curve.Evaluate(elapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.forward * value * normal * forceMultiplier);
        }

        if (Mathf.Abs(rigidbody.velocity.z) > maxVelocity)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxVelocity * Mathf.Sign(rigidbody.velocity.z));
        }

        headingVelocity = rigidbody.velocity.z;

        attackAngle = maxVelocityAngle * (headingVelocity / maxVelocity);
        body.transform.eulerAngles = new Vector3(attackAngle, body.transform.rotation.y, body.transform.rotation.z);
    }
}
