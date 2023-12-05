using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityDirect : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float velocityY = 1f;

    private Rigidbody rigidbody;

    private void ResolveComponents() => rigidbody = GetComponent<Rigidbody>();

    void Awake() => ResolveComponents();

    void FixedUpdate() => rigidbody.velocity = new Vector3(rigidbody.velocity.x, velocityY, rigidbody.velocity.z);
}
