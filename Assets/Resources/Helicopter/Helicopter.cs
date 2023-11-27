using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Helicopter
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class Helicopter : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] Transform body;
        [SerializeField] Transform rotars;

        [Header("Speed")]
        [SerializeField] AnimationCurve speedCurve;

        [Header("Rotar Speed")]
        [Range(10f, 50f)]
        [SerializeField] float rotarSpeedScaleFactor = 15f;

        [Header("Banking")]
        [SerializeField] AnimationCurve bankCurve;

        [Header("Rotation")]
        [SerializeField] AnimationCurve rotationCurve;

        [Header("Stats")]
        [Range(0f, 1000f)]
        [SerializeField] float rotarSpeed;
        [SerializeField] Vector3 velocity;

        private const float TAKEOFF_LAND_ROTAR_SPEED_SCALE_FACTOR = 50f;
        private const float ROTAR_SPEED_TAKE_OFF_THRESHOLD = 750f;
        private const float CAPPED_ROTAR_SPEED = 1000f;
        private const float MAX_ROTAR_SPEED = 1500f;

        private AudioSource audioSource;
        private Rigidbody rigidbody;
        private HelicopterInputActionMap actionMap;
        private State state;
        private Vector3 maxForce;
        // private float rotarSpeed = 0f;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();

            actionMap = new HelicopterInputActionMap();
            maxForce = -Physics.gravity * 2f;
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
        }

        void OnDisable() => actionMap.Disable();

        // Update is called once per frame
        void Update()
        {
            if (state == State.EngagePower)
            {
                float newSpeed  = rotarSpeed + (TAKEOFF_LAND_ROTAR_SPEED_SCALE_FACTOR * Time.deltaTime);

                if (Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED) < ROTAR_SPEED_TAKE_OFF_THRESHOLD)
                {
                    rotarSpeed = Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED);
                }
                else
                {
                    rotarSpeed = ROTAR_SPEED_TAKE_OFF_THRESHOLD;
                    state = State.Active;
                }
            }
            else if (state == State.Active)
            {
                var rotarSpeedAction = actionMap.Helicopter.RotarSpeed;

                if (rotarSpeedAction.IsPressed())
                {
                    var value = rotarSpeedAction.ReadValue<float>();
                    rotarSpeed = Mathf.Clamp(rotarSpeed += value * rotarSpeedScaleFactor, 0f, 1000f);
                }
            }
            else if (state == State.CutPower)
            {
                float newSpeed  = rotarSpeed - (TAKEOFF_LAND_ROTAR_SPEED_SCALE_FACTOR * Time.deltaTime);

                if (Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED) > 0)
                {
                    rotarSpeed = Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED);
                }
                else
                {
                    rotarSpeed = 0f;
                    state = State.Idle;
                }
            }

            rotars.transform.Rotate(Vector3.up * rotarSpeed * Time.deltaTime, Space.Self);
            audioSource.pitch = rotarSpeed / CAPPED_ROTAR_SPEED;
            audioSource.volume = rotarSpeed / CAPPED_ROTAR_SPEED;

            velocity = rigidbody.velocity;
        }

        void FixedUpdate()
        {
            var upForce = (rotarSpeed / MAX_ROTAR_SPEED) * maxForce;
            rigidbody.AddForce(upForce);
        }

        private void OnStartEngine(InputAction.CallbackContext context)
        {
            if (state != State.Idle) return;
            state = State.EngagePower;

            // StartCoroutine(Co_StartEngine());
        }

        // private IEnumerator Co_StartEngine()
        // {
        //     bool complete = false;

        //     do
        //     {
        //         float newSpeed  = rotarSpeed + (TAKEOFF_LAND_ROTAR_SPEED_SCALE_FACTOR * Time.deltaTime);

        //         if (Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED) < ROTAR_SPEED_TAKE_OFF_THRESHOLD)
        //         {
        //             rotarSpeed = Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED);
        //         }
        //         else
        //         {
        //             rotarSpeed = ROTAR_SPEED_TAKE_OFF_THRESHOLD;
        //             complete = true;
        //         }
                
        //         yield return null;
        //     } while (!complete);

        //     state = State.Active;
        // }

        private void OnCutEngine(InputAction.CallbackContext context)
        {
            if (state != State.Active) return;
            state = State.CutPower;

            // StartCoroutine(Co_CutEngine());
        }

        // private IEnumerator Co_CutEngine()
        // {
        //     bool complete = false;

        //     do
        //     {
        //         float newSpeed  = rotarSpeed - (TAKEOFF_LAND_ROTAR_SPEED_SCALE_FACTOR * Time.deltaTime);

        //         if (Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED) > 0)
        //         {
        //             rotarSpeed = Mathf.Clamp(newSpeed, 0f, CAPPED_ROTAR_SPEED);
        //         }
        //         else
        //         {
        //             rotarSpeed = 0f;
        //             complete = true;
        //         }

        //         yield return null;
        //     } while (!complete);

        //     state = State.Idle;
        // }
    }
}
