using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class VelocityOverTimeCalculator : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] TextMeshProUGUI velocityTextUI;
    [SerializeField] TextMeshProUGUI pVelocityTextUI;

    [Header("Config")]
    [SerializeField] float force;

    private Rigidbody rigidbody;
    private float startSec;
    private bool actioned;

    private void ResolveComponents() => rigidbody = GetComponent<Rigidbody>();

    void Awake() => ResolveComponents();

    // Update is called once per frame
    void Update()
    {
        velocityTextUI.text = $"Velocity {rigidbody.velocity.z}";

        float velocity = (force / rigidbody.mass) * (Time.time - startSec);
        // velocity = velocity * (1f - Time.deltaTime * rigidbody.drag);
        velocity = Mathf.Round(velocity * 100f) / 100f;
        pVelocityTextUI.text = $"P Velocity {velocity}";

        Debug.Log($"Velocity {rigidbody.velocity.z} P Velocity {velocity}");

    }

    void FixedUpdate()
    {
        if (!actioned)
        {
            actioned = true;

            var forceToApply = Vector3.forward * force;
            Debug.Log($"Applied force: {forceToApply}");

            rigidbody.AddForce(forceToApply);
            startSec = Time.time;
        }
    }
}
