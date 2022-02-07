using System;

using UnityEngine;

// [RequireComponent(typeof(Animator))]
public class SpeedballAvatarHand : MonoBehaviour
{
    // private const string AnimatorGripParam = "Grip";
    // private static readonly int Grip = Animator.StringToHash(AnimatorGripParam);
    // private const string AnimatorTriggerParam = "Trigger";
    // private static readonly int Trigger = Animator.StringToHash(AnimatorTriggerParam);

    // Animation
    // [SerializeField] private float animationSpeed;
    // private Animator animator;
    // private SkinnedMeshRenderer mesh;
    // private float gripTarget;
    // private float triggerTarget;
    // private float gripCurrent;
    // private float triggerCurrent;

    // Physics Movement
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    private Transform followTarget;
    private Rigidbody body;
    
    // Start is called before the first frame update
    void Start()
    {
        // Animation
        // animator = GetComponent<Animator>() as Animator;
        // mesh = GetComponentInChildren<SkinnedMeshRenderer>() as SkinnedMeshRenderer;

        // Physics Movement
        followTarget = followObject.transform;
        body = GetComponent<Rigidbody>() as Rigidbody;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.mass = 20f;

        // Teleport hands
        body.position = followTarget.position;
        body.rotation = followTarget.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // AnimateHand();
        PhysicsMove();
    }

    private void PhysicsMove()
    {
        // Position
        // var positionWithOffset  = followTarget.position + positionOffset;
        var positionWithOffset  = followTarget.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        body.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance);

        // Rotation
        var rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        body.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }

    // internal void SetGrip(float value)
    // {
    //     gripTarget = value;
    // }

    // internal void SetTrigger(float value)
    // {
    //     triggerTarget = value; 
    // }

    // private void AnimateHand()
    // {
    //     if (Math.Abs(gripCurrent - gripTarget) > 0)
    //     {
    //         gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * animationSpeed);
    //         animator.SetFloat(Grip, gripCurrent);
    //     }

    //     if (!(Math.Abs(triggerCurrent - triggerTarget) > 0)) return;

    //     triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
    //     animator.SetFloat(Trigger, triggerCurrent);
    // }

    // private void ToggleVisibility()
    // {
    //     mesh.enabled = !mesh.enabled;
    // }
}