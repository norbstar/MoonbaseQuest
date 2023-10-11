using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DotProductTest : BaseManager
{
    [Header("Components")]       
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] Transform reference;
    [SerializeField] GameObject projectile;

    [Header("Config")]
    [SerializeField] float speed = 2000f;

    private Rigidbody rigidbody;

    void Awake() => rigidbody = projectile.GetComponent<Rigidbody>();

    // Start is called before the first frame update
    // void Start() => rigidbody.AddForce(-speed, 0f, 0f);

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = end.position - start.position;
        float magnitude = direction.magnitude;
        direction.Normalize();

        Vector3 difference = reference.position - start.position;
        var dotProduct = Vector3.Dot(difference, direction) / magnitude;
        var force = transform.forward * (dotProduct * speed);

        Log($"Update[1] Normalized Direction: {direction} Difference: {difference} Dot Product: {dotProduct} Force: {force}");
    }
}
