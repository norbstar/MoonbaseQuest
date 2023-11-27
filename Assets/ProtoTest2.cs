using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class ProtoTest2 : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform body;

    [Header("Global")]
    [Range(0f, 20f)]
    [SerializeField] float maxGlobalVelocity = 10f;

    [Header("Heading")]
    [SerializeField] InputAction headingAction;
    [SerializeField] AnimationCurve headingcurve;
    [Range(10f, 200f)]
    [SerializeField] float headingMultiplier = 100f;
    [Range(5f, 20f)]
    [SerializeField] float maxHeadingAngle = 15f;

    [Header("Rotation")]
    [SerializeField] InputAction rotationAction;
    [Range(50f, 250f)]
    [SerializeField] float rotationMultiplier = 100f;

    [Header("Banking")]
    [SerializeField] InputAction bankingAction;
    [SerializeField] AnimationCurve bankingcurve;
    [Range(10f, 200f)]
    [SerializeField] float bankingMultiplier = 100f;
    [Range(5f, 20f)]
    [SerializeField] float maxBankingAngle = 15f;

    [Header("Stats")]
    // [SerializeField] Vector3 localHeadingVelocity;
    [SerializeField] Vector3 velocity;
    [SerializeField] float headingAttackAngle;
    [SerializeField] float bankingAttackAngle;

    private Rigidbody rigidbody;
    private float headingStartSec, headingElapsedTimeSec;
    private bool headingIsPressed;
    private float bankingStartSec, bankingElapsedTimeSec;
    private bool bankingIsPressed;

    void Awake() => rigidbody = GetComponent<Rigidbody>();

    void OnEnable()
    {
        headingAction.Enable();
        rotationAction.Enable();
        bankingAction.Enable();
    }

    void OnDisable()
    {
        headingAction.Disable();
        rotationAction.Disable();
        bankingAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (headingAction.triggered)
        {
            headingStartSec = Time.time;
            headingElapsedTimeSec = 0f;
        }

        headingIsPressed = headingAction.IsPressed();

        if (bankingAction.triggered)
        {
            bankingStartSec = Time.time;
            bankingElapsedTimeSec = 0f;
        }

        bankingIsPressed = bankingAction.IsPressed();

        var rotation = rotationAction.ReadValue<float>();
        var rotationSpeed = rotation * rotationMultiplier;
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }

    void FixedUpdate()
    {
#if false
        if (headingIsPressed)
        {
            var value = headingAction.ReadValue<float>();
            elapsedTimeSec += Time.deltaTime;
            var normal = curve.Evaluate(elapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.forward * value * normal * forceMultiplier);
        }

        if (Mathf.Abs(rigidbody.velocity.z) > maxVelocity)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxVelocity * Mathf.Sign(rigidbody.velocity.z));
        }

        worldHeadingVelocity = rigidbody.velocity;
        attackAngle = maxHeadingAngle * (worldHeadingVelocity.z / maxHeadingVelocity);
#endif

#if true
        if (headingIsPressed)
        {
            var value = headingAction.ReadValue<float>();
            headingElapsedTimeSec += Time.deltaTime;
            var normal = headingcurve.Evaluate(headingElapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.forward * value * normal * headingMultiplier);
        }

        if (bankingIsPressed)
        {
            var value = bankingAction.ReadValue<float>();
            bankingElapsedTimeSec += Time.deltaTime;
            var normal = bankingcurve.Evaluate(bankingElapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.right * value * normal * bankingMultiplier);
        }

        if (Mathf.Abs(rigidbody.velocity.magnitude) > maxGlobalVelocity)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxGlobalVelocity);
        }

        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

        headingAttackAngle = maxHeadingAngle * (localVelocity.z / maxGlobalVelocity);
        bankingAttackAngle = maxBankingAngle * (-localVelocity.x / maxGlobalVelocity);

        velocity = rigidbody.velocity;
#endif

#if false
        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

        if (headingIsPressed)
        {
            var value = headingAction.ReadValue<float>();
            elapsedTimeSec += Time.deltaTime;
            var normal = curve.Evaluate(elapsedTimeSec);
            rigidbody.AddRelativeForce(Vector3.forward * value * normal * forceMultiplier);
        }

        if (Mathf.Abs(localVelocity.z) > maxHeadingVelocity)
        {            
            localVelocity = new Vector3(localVelocity.x, localVelocity.y, Mathf.Sign(localVelocity.z) * maxHeadingVelocity);
            rigidbody.velocity = transform.TransformDirection(localVelocity);
        }

        localHeadingVelocity = localVelocity;
        worldHeadingVelocity = rigidbody.velocity;
        attackAngle = maxHeadingAngle * (localHeadingVelocity.z / maxHeadingVelocity);
#endif

        body.transform.localEulerAngles = new Vector3(headingAttackAngle, body.transform.localRotation.y, bankingAttackAngle);
    }
}
