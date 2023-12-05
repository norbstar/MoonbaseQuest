using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class VelocityCalculator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI velocityTextUI;

    [Header("Config")]
    [SerializeField] float force;

    private Rigidbody rigidbody;
    private bool actioned;

    private void ResolveComponents() => rigidbody = GetComponent<Rigidbody>();

    void Awake() => ResolveComponents();

    // Update is called once per frame
    void Update() => velocityTextUI.text = $"{rigidbody.velocity.z}";

    void FixedUpdate()
    {
        if (!actioned)
        {
            actioned = true;

#if false
            float velocity = (force * Time.fixedDeltaTime) / rigidbody.mass;

            velocity = Mathf.Round(velocity * 100f) / 100f;
            Debug.Log($"Projected velocity: {velocity}");
#endif

            var forceToApply = Vector3.forward * force;
            Debug.Log($"Applied force: {forceToApply}");

            rigidbody.AddForce(forceToApply);
        }
    }
}
