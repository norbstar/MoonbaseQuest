using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(BasicPhysicsComponent))]
[RequireComponent(typeof(AudioSource))]
public class CounterGravityWithVelocityTest : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform rotars;

    [Header("Config")]
    [Range(0f, 1f)]
    [SerializeField] float normalisedVelocityY;

    [Header("Stats")]
    [SerializeField] float stableVelocityY;
    [SerializeField] float maxVelocityY;
    [SerializeField] float velocityY;
    
    [SerializeField] Vector3 velocity;

    public const float NOMINAL_ROTAR_SPEED = 750f;
    public const float MAX_ROTAR_SPEED = 1000f;

    private BasicPhysicsComponent physicsComponent;
    private AudioSource audioSource;
    private float rotarSpeed;

    private void ResolveComponents()
    {
        physicsComponent = GetComponent<BasicPhysicsComponent>();
        audioSource = GetComponent<AudioSource>();
    }

    void Awake() => ResolveComponents();

    // Start is called before the first frame update
    void Start()
    {
        audioSource.Play();
        stableVelocityY = (-Physics.gravity.y / physicsComponent.Rigidbody.mass) * Time.fixedDeltaTime;
        maxVelocityY = stableVelocityY * (MAX_ROTAR_SPEED / NOMINAL_ROTAR_SPEED);
        normalisedVelocityY = stableVelocityY / maxVelocityY;
    }

    // Update is called once per frame
    void Update()
    {
        velocityY = maxVelocityY * normalisedVelocityY;
        rotarSpeed = MAX_ROTAR_SPEED * (velocityY / maxVelocityY);
        rotars.transform.Rotate(Vector3.up * rotarSpeed * Time.deltaTime, Space.Self);
        velocity = physicsComponent.Rigidbody.velocity;
        audioSource.pitch = rotarSpeed / MAX_ROTAR_SPEED;
        audioSource.volume = rotarSpeed / MAX_ROTAR_SPEED;
    }

    void FixedUpdate() => physicsComponent.Rigidbody.velocity = new Vector3(0f, velocityY, 0f);
}
