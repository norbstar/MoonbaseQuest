using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CubeForceCurveTest : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] AnimationCurve curve;

    [Header("Velocity")]
    [Range(10f, 500f)]
    [SerializeField] float forceMultiplier = 100f;
    [Range(10f, 50f)]
    [SerializeField] float maxVelocity = 15f;
    [Range(5f, 25f)]
    [SerializeField] float maxVelocityAngle = 15f;

    [Header("Bindings")]
    [SerializeField] InputAction action;

    [Header("Stats")]
    [SerializeField] float velocity = 0f;
    [SerializeField] float angle = 0f;

    // private const float MAX_FORCE = 10f;
    // private const float KICK_FORCE = 20f;

    private Rigidbody rigidbody;
    private float startSec, elapsedTimeSec;
    private bool isPressed;
    // private bool addKick;

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
            
            // addKick = true;
        }

        isPressed = action.IsPressed();

        // if (rigidbody.velocity > maxVelocity)
        // {
        //     Vector3.ClampMagnitude(rigidbody, maxVelocity);
        // }

        if (rigidbody.velocity.z > maxVelocity)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxVelocity);
        }

        velocity = rigidbody.velocity.z;

        float angle = maxVelocityAngle * (velocity / maxVelocity);
        transform.eulerAngles = new Vector3(angle, transform.rotation.y, transform.rotation.z);
    }

    void FixedUpdate()
    {
        // if (addKick)
        // {
        //     if (rigidbody.velocity.z > 0f)
        //     {
        //         rigidbody.AddRelativeForce(-Vector3.forward * KICK_FORCE * forceMultiplier);
        //     }
        //     else
        //     {
        //         rigidbody.AddRelativeForce(Vector3.forward * KICK_FORCE * forceMultiplier);
        //     }

        //     addKick = false;
        // }

        if (isPressed)
        {
            var value = action.ReadValue<float>();
            elapsedTimeSec += Time.deltaTime;
            var normal = curve.Evaluate(elapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.forward * value * normal * forceMultiplier);
        }
    }
}
