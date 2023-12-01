using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Helicopter
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class CoastGuardHelicopter : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] Transform propellerHub;
        [SerializeField] Transform propellers;
        
        [Header("Settings")]
        [Tooltip("Speed Multiplier")]
        [Range(100f, 500f)]
        [SerializeField] float speedMultiplier = 400f;
        
        [Tooltip("Max Speed")]
        [SerializeField] float maxSpeed = 100f;

        [Tooltip("Rotation Speed Multiplier")]
        [Range(50f, 250f)]
        [SerializeField] float rotationSpeedMultiplier = 100f;

        [Tooltip("Max Rotation Speed")]
        [SerializeField] float maxRotationSpeed = 100f;

        // [Tooltip("The rotar speed")]
        // [Range(0f, 1000f)]
        // [SerializeField] float rotarSpeed = 0f;

        private const float ROTAR_SPEED_SCALE_FACTOR = 2f;
        private const float TAKE_OFF_ROTAR_SPEED_THRESHOLD = 750f;
        private const float CAPPED_ROTAR_SPEED = 1000f;
        private const float MAX_ROTAR_SPEED = 1500f;

        private AudioSource audioSource;
        private Rigidbody rigidbody;
        private HelicopterInputActionMap actionMap;
        private State state;
        private Vector3 maxForce;
        private float rotarSpeed = 0f;
        // private float speed;
        // private float rotationSpeed;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();

            actionMap = new HelicopterInputActionMap();
            maxForce = -Physics.gravity * 2f;

            // Debug.Log($"Gravity: {Physics.gravity}");
            // Debug.Log($"Max Force: {maxForce}");
        }

        // Start is called before the first frame update
        void Start()
        {
            audioSource.Play();
            state = State.Idle;
        }

        void OnEnable()
        {
            actionMap.Enable();
            actionMap.Helicopter.EngagePower.performed += OnStartEngine;
            actionMap.Helicopter.CutPower.performed += OnCutEngine;
            // actionMap.Helicopter.Speed.performed += OnSpeed;
        }

        void OnDisable() => actionMap.Disable();

        // Update is called once per frame
        void Update()
        {
            // propellers.transform.Rotate(Vector3.up * Time.deltaTime * rotarSpeed, Space.Self);
            // audioSource.pitch = rotarSpeed / 1000f;

            // var startEngine = actionMap.Helicopter.StartEngine;
            // Debug.Log($"{startEngine.triggered}");

            if (state == State.EngagingPower)
            {
                if (Mathf.Clamp(rotarSpeed + ROTAR_SPEED_SCALE_FACTOR, 0f, CAPPED_ROTAR_SPEED) < TAKE_OFF_ROTAR_SPEED_THRESHOLD)
                {
                    rotarSpeed = Mathf.Clamp(rotarSpeed + ROTAR_SPEED_SCALE_FACTOR, 0f, CAPPED_ROTAR_SPEED);
                }
                else
                {
                    rotarSpeed = TAKE_OFF_ROTAR_SPEED_THRESHOLD;
                    state = State.Active;
                }
            }
            else if (state == State.Active)
            {
                var rotarSpeedAction = actionMap.Helicopter.RotarSpeed;

                if (rotarSpeedAction.IsPressed())
                {
                    var value = rotarSpeedAction.ReadValue<float>();
                    rotarSpeed = Mathf.Clamp(rotarSpeed += value * ROTAR_SPEED_SCALE_FACTOR, 0f, 1000f);
                }
            }
            else if (state == State.CuttingPower)
            {
                if (Mathf.Clamp(rotarSpeed - ROTAR_SPEED_SCALE_FACTOR, 0f, CAPPED_ROTAR_SPEED) > 0)
                {
                    rotarSpeed = Mathf.Clamp(rotarSpeed - ROTAR_SPEED_SCALE_FACTOR, 0f, CAPPED_ROTAR_SPEED);
                }
                else
                {
                    rotarSpeed = 0f;
                    state = State.Idle;
                }
            }

            var upForce = (rotarSpeed / MAX_ROTAR_SPEED) * (Time.deltaTime * maxForce);
            rigidbody.AddForce(upForce);

#if false
            var move = actionMap.Helicopter.Move.ReadValue<Vector2>();

            // speed += move.y * 2f;
            // transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

            var speed = move.y * speedMultiplier;
            rigidbody.AddForce(transform.forward * Time.deltaTime * speed);
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);

            // Set rotation speed
            var rotationSpeed = move.x * rotationSpeedMultiplier;
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.Self);

            // Set propeller speed
            propellers.transform.Rotate(Vector3.up * Time.deltaTime * rotarSpeed, Space.Self);

            // Set audio pitch relative to propeller speed
            audioSource.pitch = rotarSpeed / CAPPED_ROTAR_SPEED;
#endif
        }

#if false
        void FixedUpdate()
        {
            propellers.transform.Rotate(Vector3.up * Time.deltaTime * rotarSpeed, Space.Self);
            audioSource.pitch = rotarSpeed / CAPPED_ROTAR_SPEED;

            // rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            var upForce = (rotarSpeed / MAX_ROTAR_SPEED) * maxForce;
            rigidbody.AddForce(upForce);
            // Debug.Log($"Velocity: {rigidbody.velocity.y}");
            // rigidbody.constraints = RigidbodyConstraints.None;

            var move = actionMap.Helicopter.Move.ReadValue<Vector2>();
            var speed = move.y * 4f;
            rigidbody.AddForce(transform.forward * speed);
        }
#endif

        // if (context.started)
        // {
        //     Debug.Log("Started");
        // }
        // if (context.performed)
        // {
        //     Debug.Log("Performed");
        // }
        // else if (context.canceled)
        // {
        //     Debug.Log("Cancelled");
        // }

        private void OnStartEngine(InputAction.CallbackContext context)
        {
            if (state != State.Idle) return;
            state = State.EngagingPower;
        }

        private void OnCutEngine(InputAction.CallbackContext context)
        {
            if (state != State.Active) return;
            state = State.CuttingPower;
        }

        // private void OnSpeed(InputAction.CallbackContext context)
        // {
        //     var speed = context.ReadValue<Vector2>().y;
        //     rotarSpeed = Mathf.Clamp(rotarSpeed += speed * ROTAR_SPEED_SCALE_FACTOR, 0f, 1000f);
        // }

        // private IEnumerator Co_StartEngine()
        // {
        //     yield return null;
        // }

        // private IEnumerator Co_StopEngine()
        // {
        //     yield return null;
        // }
    }
}