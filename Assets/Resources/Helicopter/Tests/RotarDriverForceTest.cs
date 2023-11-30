using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter.Tests
{
    public class RotarDriverForceTest : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] Transform rotars;

        [Header("Config")]
        [SerializeField] float torque = 10f;

        private Rigidbody rigidbody;

        private void ResolveComponents() => rigidbody = rotars.GetComponent<Rigidbody>();

        void Awake() => ResolveComponents();
        
        void FixedUpdate() => rigidbody.AddTorque(transform.up * torque, ForceMode.VelocityChange);
    }
}