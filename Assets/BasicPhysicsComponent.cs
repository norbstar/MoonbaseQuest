using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicPhysicsComponent : MonoBehaviour
{
    private Rigidbody rigidbody;

    private void ResolveComponents() => rigidbody = GetComponent<Rigidbody>();

    void Awake() => ResolveComponents();

    public Rigidbody Rigidbody { get { return rigidbody; } }

    public Vector3 Velocity { get { return rigidbody.velocity; } }

    public Vector3 AngularVelocity { get { return rigidbody.angularVelocity; } }

    public float Mass { get { return rigidbody.mass; } }

    public float Drag { get { return rigidbody.drag; } }

    public float AngularDrag { get { return rigidbody.angularDrag; } }
}
