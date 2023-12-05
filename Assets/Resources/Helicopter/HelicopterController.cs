using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

namespace Helicopter
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(HelicopterEngagePowerCurveFunction))]
    [RequireComponent(typeof(HelicopterCutPowerCurveFunction))]
    [RequireComponent(typeof(HelicopterStabiliseElevationCurveFunction))]
    public class HelicopterController : MonoBehaviour, IHelicopterCurveFunctionInput
    {
        [Header("Parts")]
        [SerializeField] Transform body;
        [SerializeField] Transform rotars;

        [Header("UI")]
        [SerializeField] TextMeshProUGUI stateTextUI;
        [SerializeField] TextMeshProUGUI altitudeTextUI;
        [SerializeField] TextMeshProUGUI lastAltitudeTextUI;
        [SerializeField] TextMeshProUGUI velocityTextUI;
        [SerializeField] TextMeshProUGUI velocityDeltaTextUI;
        [SerializeField] TextMeshProUGUI rotarSpeedTextUI;

        [Header("Stats")]
        [Range(0f, 1000f)]
        [SerializeField] float rotarSpeed;
        [SerializeField] Vector3 velocity;
    
        public const float ROTAR_SPEED_SCALE_FACTOR = 15f;
        public const float ROTAR_SPEED_LEVEL_THRESHOLD = 750f;
        public const float CAPPED_ROTAR_SPEED = 1000f;
        public const float MAX_ROTAR_SPEED = 1500f;
        public const float MIN_ALTITUDE = 2.65f;
        public const float MAX_ALTITUDE = 50f;

        private Rigidbody rigidbody;
        private AudioSource audioSource;
        private HelicopterEngagePowerCurveFunction engagePowerCurveFn;
        private HelicopterStabiliseElevationCurveFunction stabiliseElevationCurveFn;
        private HelicopterStabiliseDescentCurveFunction stabiliseDescentCurveFn;
        private HelicopterCutPowerCurveFunction cutPowerCurveFn;
        private HelicopterInputActionMap actionMap;
        private State state;
        private Vector3 maxForce;
        private float lastElevation, lastVelocity;
        private bool isDescending;

        private void ResolveComponents()
        {
            rigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            engagePowerCurveFn = GetComponent<HelicopterEngagePowerCurveFunction>();
            cutPowerCurveFn = GetComponent<HelicopterCutPowerCurveFunction>();
            stabiliseElevationCurveFn = GetComponent<HelicopterStabiliseElevationCurveFunction>();
            stabiliseDescentCurveFn = GetComponent<HelicopterStabiliseDescentCurveFunction>();
        }

        void Awake()
        {
            ResolveComponents();
            actionMap = new HelicopterInputActionMap();
            maxForce = -Physics.gravity * 2f;

            engagePowerCurveFn.Init(this);
            stabiliseElevationCurveFn.Init(this);
            stabiliseDescentCurveFn.Init(this);
            cutPowerCurveFn.Init(this);
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
            actionMap.Helicopter.EngagePower.performed += OnEngagePower;
            actionMap.Helicopter.StabiliseElevation.performed += OnStabiliseElevation;
            actionMap.Helicopter.StabiliseDescent.performed += OnStabiliseDescent;
            actionMap.Helicopter.CutPower.performed += OnCutPower;
        }

        void OnDisable() => actionMap.Disable();

        // Update is called once per frame
        void Update()
        {
            var rotarSpeedAction = actionMap.Helicopter.RotarSpeed;

            if (rotarSpeedAction.IsPressed())
            {
                switch (state)
                {
                    case State.StabilisingElevation:
                        state = State.Active;
                        break;
                }
            }

            switch (state)
            {
                case State.EngagingPower:
                    OnEngagingPower();
                    break;

                case State.Active:
                    OnActive();
                    break;

                case State.StabilisingElevation:
                    OnStabilisingElevation();
                    break;

                case State.StabilisingDescent:
                    OnStabilisingDescent();
                    break;

                case State.CuttingPower:
                    OnCuttingPower();
                    break;    
            }

            rotars.transform.Rotate(Vector3.up * rotarSpeed * Time.deltaTime, Space.Self);

            audioSource.pitch = rotarSpeed / CAPPED_ROTAR_SPEED;
            audioSource.volume = rotarSpeed / CAPPED_ROTAR_SPEED;

            velocity = rigidbody.velocity;

            if (transform.position.y > MAX_ALTITUDE)
            {
                transform.position = new Vector3(transform.position.x, MAX_ALTITUDE, transform.position.z);
            }

            stateTextUI.text = $" State: {state.ToString()}";
            altitudeTextUI.text = $"Altitude: {transform.position.z}";
            lastAltitudeTextUI.text = $"Last Altitude: {transform.position.y}";
            velocityTextUI.text = $"Velocity: {rigidbody.velocity}";
            velocityDeltaTextUI.text = $"Velocity Delta: {rigidbody.velocity.y - lastVelocity}";
            rotarSpeedTextUI.text = $"Rotar Speed: {rotarSpeed}";

            lastElevation = transform.position.y;
            lastVelocity = rigidbody.velocity.y;
        }

        void FixedUpdate()
        {
            var upForce = (rotarSpeed / MAX_ROTAR_SPEED) * maxForce;
            rigidbody.AddForce(upForce * 0.1f, ForceMode.VelocityChange);
        }

        private void OnEngagePower(InputAction.CallbackContext context)
        {
            if (state != State.Idle) return;

            engagePowerCurveFn.Exec();
            state = State.EngagingPower;
        }

        private void OnEngagingPower()
        {
            rotarSpeed = engagePowerCurveFn.Get();

            if (rotarSpeed == ROTAR_SPEED_LEVEL_THRESHOLD)
            {
                state = State.Active;
            }
        }

        private void OnActive()
        {
            var rotarSpeedAction = actionMap.Helicopter.RotarSpeed;

            if (rotarSpeedAction.IsPressed())
            {
                var value = rotarSpeedAction.ReadValue<float>();
                rotarSpeed = Mathf.Clamp(rotarSpeed += value * ROTAR_SPEED_SCALE_FACTOR, 0f, 1000f);
            }
        }

        private void OnStabiliseElevation(InputAction.CallbackContext context)
        {
            if (state != State.Active) return;

            state = State.StabilisingElevation;
        }

        private void OnStabilisingElevation()
        {
            rotarSpeed = stabiliseElevationCurveFn.Get();

            if (transform.position.y == lastElevation)
            {
                OnStabilised();
            }
        }

        private void OnStabilised()
        {
            rotarSpeed = ROTAR_SPEED_LEVEL_THRESHOLD;

            if (isDescending)
            {
                isDescending = false;
                state = State.StabilisingDescent;
                cutPowerCurveFn.Exec();
            }
            else
            {
                state = State.Active;
            }
        }

        
        private void OnStabiliseDescent(InputAction.CallbackContext context)
        {
            switch (state)
            {
                case State.StabilisingElevation:
                    state = State.Active;
                    break;
            }

            if (state != State.Active) return;

            isDescending = true;
            state = State.StabilisingElevation;
            stabiliseDescentCurveFn.Exec();
        }

        private void OnStabilisingDescent()
        {
            rotarSpeed = stabiliseDescentCurveFn.Get();

            if (Mathf.Abs(transform.position.y - MIN_ALTITUDE) < 0.1f)
            {
                rotarSpeed = ROTAR_SPEED_LEVEL_THRESHOLD;
                transform.position = new Vector3(transform.position.x, MIN_ALTITUDE, transform.position.z);
                state = State.Active;
            }
        }

        private void OnCutPower(InputAction.CallbackContext context)
        {
            if (state != State.Active) return;

            cutPowerCurveFn.Exec();
            state = State.CuttingPower;
        }

        private void OnCuttingPower()
        {
            rotarSpeed = cutPowerCurveFn.Get();

            if (Mathf.Abs(transform.position.y - MIN_ALTITUDE) < 0.1f)
            {
                rotarSpeed = 0f;
                transform.position = new Vector3(transform.position.x, MIN_ALTITUDE, transform.position.z);
                state = State.Idle;
            }
        }

        private float GetRotarSpeed() => rotarSpeed;
        
        private float GetAltitude() => transform.position.y;
        
        private Vector3 GetVelocity() => rigidbody.velocity;

        private Vector3 GetPosition() => transform.position;

        public HelicopterProperties GetProperties()
        {
            return new HelicopterProperties
            {
                rotarSpeed = GetRotarSpeed(),
                altitude = GetAltitude(),
                velocity = GetVelocity(),
                position = GetPosition()
            };
        }
    }
}