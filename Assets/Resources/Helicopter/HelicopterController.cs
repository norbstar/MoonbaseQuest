using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Stats")]
        [Range(0f, 1000f)]
        [SerializeField] float rotarSpeed;
        [SerializeField] Vector3 velocity;
    
        private const float ROTAR_SPEED_SCALE_FACTOR = 15f;
        private const float ROTAR_SPEED_TAKE_OFF_THRESHOLD = 750f;
        private const float CAPPED_ROTAR_SPEED = 1000f;
        private const float MAX_ROTAR_SPEED = 1500f;

        private AudioSource audioSource;
        private Rigidbody rigidbody;
        private HelicopterEngagePowerCurveFunction engagePowerCurveFn;
        private HelicopterCutPowerCurveFunction cutPowerCurveFn;
        private HelicopterStabiliseElevationCurveFunction stabiliseElevationCurveFn;
        private HelicopterInputActionMap actionMap;
        private State state;
        private Vector3 maxForce;

        private void ResolveComponents()
        {
            rigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            engagePowerCurveFn = GetComponent<HelicopterEngagePowerCurveFunction>();
            cutPowerCurveFn = GetComponent<HelicopterCutPowerCurveFunction>();
            stabiliseElevationCurveFn = GetComponent<HelicopterStabiliseElevationCurveFunction>();
        }

        void Awake()
        {
            ResolveComponents();
            actionMap = new HelicopterInputActionMap();
            maxForce = -Physics.gravity * 2f;

            engagePowerCurveFn.Init(this);
            cutPowerCurveFn.Init(this);

            // Debug.Log($"Min: {-Physics.gravity.y} Max: {Physics.gravity.y}");
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
            actionMap.Helicopter.CutPower.performed += OnCutPower;
            actionMap.Helicopter.StabiliseElevation.performed += OnStabiliseElevation;
        }

        void OnDisable() => actionMap.Disable();

        // Update is called once per frame
        void Update()
        {
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

                case State.CuttingPower:
                    OnCuttingPower();
                    break;    
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

        private void OnEngagePower(InputAction.CallbackContext context)
        {
            if (state != State.Idle) return;

            engagePowerCurveFn.Exec();
            state = State.EngagingPower;
        }

        private void OnEngagingPower()
        {
            rotarSpeed = engagePowerCurveFn.Get();

            if (rotarSpeed == ROTAR_SPEED_TAKE_OFF_THRESHOLD)
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
            var clampedYVelocity = Mathf.Clamp(rigidbody.velocity.y, -10f, 10f);
            var normalisedValue = clampedYVelocity.Remap(-10f, 10f, -1f, 1f);
            rotarSpeed = Mathf.Clamp(stabiliseElevationCurveFn.Get(normalisedValue), 0f, CAPPED_ROTAR_SPEED);
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

            if (rotarSpeed == 0f)
            {
                state = State.Idle;
            }
        }

        public float GetRotarSpeed() => rotarSpeed;
        
        public float GetElevation() => transform.position.y;
        
        public Vector3 GetVelcoity() => rigidbody.velocity;
    }
}