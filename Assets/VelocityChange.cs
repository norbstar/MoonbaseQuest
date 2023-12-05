using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityChange : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float velocityY = 1f;

    private Rigidbody rigidbody;

    private void ResolveComponents() => rigidbody = GetComponent<Rigidbody>();

    void Awake() => ResolveComponents();

    void FixedUpdate() => rigidbody.AddForce(Vector3.up * velocityY, ForceMode.VelocityChange);
}
