// using System.Collections;
using System.Collections.Generic;
// using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interactables.Bow
{
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(Animator))]
    public class BowInteractableManager : FocusableInteractableManager
    {
        // private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] OnProximityHandler proximityHandler;
        [SerializeField] OnTriggerHandler actuationHandler;
        [SerializeField] LineRendererHelper lineRendererHelper;
        [SerializeField] Transform start;
        [SerializeField] Transform end;
        [SerializeField] Arrow arrow;
        // [SerializeField] float pullRange = 0.25f;

        [Header("Config")]
        [SerializeField] bool enableHaptics;
        [SerializeField] bool showLineRenderer;

        // [SerializeField] GameObject target;

        private GameObject bowParts;
        private Animator animator;
        private HandController leftController, rightController, opposingController, triggeredBy;
        // private InputDevice leftDevice, rightDevice;
        private Status status;

        protected override void Awake()
        {
            base.Awake();

            bowParts = transform.GetChild(0).gameObject;
            animator = GetComponent<Animator>();
            status = Status.None;
            // arrow.Hide();
            arrow.Alpha = 0f;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (TryGet.TryGetControllers(out List<HandController> controllers))
            {
                foreach (HandController controller in controllers)
                {
                    // var inputDevice = controller.GetComponent<XRController>().inputDevice;

                    if ((int) controller.Characteristics == (int) HandController.LeftHand)
                    {
                        leftController = controller;
                        // leftDevice = inputDevice;
                    }
                    else if ((int) controller.Characteristics == (int) HandController.RightHand)
                    {
                        rightController = controller;
                        // rightDevice = inputDevice;
                    }
                }
            }
        }

        void OnEnable()
        {
            HandController.InputChangeEventReceived += OnActuation;
            actuationHandler.EventReceived += OnActuatorEvent;
            proximityHandler.EventReceived += OnProximityHandler;
        }

        void OnDisable()
        {
            HandController.InputChangeEventReceived -= OnActuation;
            actuationHandler.EventReceived -= OnActuatorEvent;
            proximityHandler.EventReceived -= OnProximityHandler;
        }

        private float CalculatePull()
        {
            // float distance = Vector3.Distance(start.position, triggeredBy.transform.position);
            // distance = Mathf.Clamp01(distance / pullRange);
            // float blend = Mathf.Clamp01(distance);

            float pullRange = Vector3.Distance(start.position, end.position);
            float distance = Vector3.Distance(start.position, triggeredBy.transform.position);
            distance = Mathf.Clamp01(distance / pullRange);
            float blend = Mathf.Clamp01(distance);
            
            // Vector3 direction = end.position - start.position;
            // float magnitude = direction.magnitude;
            // direction.Normalize();
            // Vector3 difference = triggeredBy.transform.position - start.position;
            // float blend = Vector3.Dot(difference, direction) / magnitude;

            return blend;
        }

        // Update is called once per frame
        void Update()
        {
            // Log($"{gameObject.name} {className} Update Status: {status}");

            // actuationHandler.gameObject.SetActive(IsHeld);

            var enableTracking = status.HasFlag(Status.IsHeld) && status.HasFlag(Status.Actuating);

            if (enableTracking)
            {
                if (showLineRenderer)
                {
                    lineRendererHelper.ConfigLine(transform.position, triggeredBy.transform.position);
                }

                // var lookPos = triggeredBy.transform.position - transform.position;
                // var rotation = Quaternion.LookRotation(lookPos);
                // transform.rotation = rotation;

                // transform.LookAt(triggeredBy.transform.position);
                transform.LookAt(transform.position - (triggeredBy.transform.position - transform.position));
                
                // Quaternion rotation = transform.localRotation;
                // rotation.z = triggeredBy.transform.localRotation.z;
                // transform.localRotation = rotation;

                // Log($"Local Euler: {IsHeldBy.transform.localEulerAngles}");
                // Log($"Local Rotation: {IsHeldBy.transform.localRotation}");

                // Quaternion rotation = bowParts.transform.localRotation;
                // rotation.z = IsHeldBy.transform.localRotation.z;
                // bowParts.transform.localRotation = rotation;

                Vector3 rotation = bowParts.transform.localEulerAngles;
                rotation.z = IsHeldBy.transform.localEulerAngles.z;
                bowParts.transform.localEulerAngles = rotation;

                var blend = CalculatePull();
                animator.SetFloat("Blend", blend);

                if (enableHaptics)
                {
                    triggeredBy.SetImpulse(0.1f);
                }
            }
            else
            {
                animator.SetFloat("Blend", 0f);

                if (status.HasFlag(Status.IsHeld))
                {
                    transform.rotation = IsHeldBy.transform.rotation;
                }
            }

            lineRendererHelper.Active = enableTracking;

            // transform.LookAt(target.transform.position);
            // transform.LookAt(transform.position - (target.transform.position - transform.position));
        }

        private void AddStatus(Status status) => this.status |= status;

        private void RevokeStatus(Status status) => this.status &= ~status;

        private void FireArrow()
        {
            if (!enableHaptics) return;
            triggeredBy.SetImpulse(0.5f);
        }

        // private IEnumerator Co_FireArrow(HandController controller)
        // {
        //     // Log($"{gameObject.name} {className} Co_FireArrow Controller: {controller.name}");

        //     yield return Co_HideArrow();
        //     RevokeStatus(Status.Actuating);
        //     controller.ShowModel();

        //     FireArrow();
        //     AddStatus(Status.Firing);
        // }

        private void FireArrow(HandController controller)
        {
            // Log($"{gameObject.name} {className} Co_FireArrow Controller: {controller.name}");

            // arrow.Hide();
            arrow.Alpha = 0f;
            RevokeStatus(Status.Actuating);
            controller.ShowModel();

            FireArrow();
            AddStatus(Status.Firing);
            bowParts.transform.localEulerAngles = Vector3.zero;
        }

        private void OnTriggering(bool canActuate, HandController controller)
        {
            // Log($"{gameObject.name} {className} OnTriggering CanActuate: {canActuate} Controller: {controller.name}");

            if (canActuate)
            {
                AddStatus(Status.Actuating);
                arrow.Alpha = 1f;
                controller.HideModel();
            }
        }

        private void OnReleasing(bool actuating, HandController controller)
        {
            // Log($"{gameObject.name} {className} OnReleasing Actuating: {actuating} Controller: {controller.name}");

            if (actuating)
            {
                // StartCoroutine(Co_FireArrow(controller));
                FireArrow(controller);
            }
        }

        private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        {
            // Log($"{gameObject.name} {className} OnActuation Controller: {controller.name} Actuation: {actuation}");

            if (!IsHeld) return;

            var isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

            var canActuate = status.HasFlag(Status.CanActuate);
            var actuating = status.HasFlag(Status.Actuating);

            if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
            {
                if ((int) characteristics == (int) HandController.RightHand)
                {
                    if (isTriggering)
                    {
                        OnTriggering(canActuate, rightController);
                    }
                    else
                    {
                        OnReleasing(actuating, rightController);
                    }
                }
            }
            else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
            {
                if ((int) characteristics == (int) HandController.LeftHand)
                {
                    if (isTriggering)
                    {
                        OnTriggering(canActuate, leftController);
                    }
                    else
                    {
                        OnReleasing(actuating, leftController);
                    }
                }
            }
        }

        private HandController GetOpposingController(HandController controller) => ((int) controller.Characteristics == (int) HandController.LeftHand) ? rightController : leftController;

        private void ShowHands()
        {
            leftController.ShowModel();
            rightController.ShowModel();
        }

        public override void OnHoldStatusChange(bool isHeld, HandController isHeldBy)
        {
            // Log($"{gameObject.name} {className} OnHoldStatusChange IsHeld: {isHeld} IsHeldBy: {isHeldBy}");

            if (isHeld)
            {
                isHeldBy.HideModel();
                opposingController = GetOpposingController(isHeldBy);
                AddStatus(Status.IsHeld);
            }
            else
            {
                // if (arrow.IsVisible)
                // {
                //     StartCoroutine(Co_HideArrow());
                // }

                bowParts.transform.localEulerAngles = Vector3.zero;
                // arrow.Hide();
                ShowHands();
                arrow.Alpha = 0f;
                opposingController = null;
                status = Status.None;
            }
        }

        // private IEnumerator Co_ShowArrow()
        // {
        //     Log($"{gameObject.name} {className} Co_ShowArrow");

        //     yield return arrow?.Co_Show();
        // }

        // private IEnumerator Co_PrepActuation(HandController controller)
        // {
        //     Log($"{gameObject.name} {className} Co_PrepActuation Controller: {controller.name}");

        //     yield return Co_ShowArrow();
        //     AddStatus(Status.CanActuate);
        //     triggeredBy = controller;
        // }

        private void PrepActuation(HandController controller)
        {
            // Log($"{gameObject.name} {className} Co_PrepActuation Controller: {controller.name}");

            // actuationHandler.gameObject.SetActive(false);
            triggeredBy = controller;
            AddStatus(Status.CanActuate);
        }

        private void OnActuatorEnter(GameObject trigger)
        {
            // Log($"{gameObject.name} {className} OnActuatorEnter Collider: {trigger.name}");

            if (!IsHeld || !trigger.tag.Equals("Hand")) return;

            bool canActuate = false;
            
            var controller = trigger.GetComponent<HandController>();

            if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
            {
                if ((int) controller.Characteristics == (int) HandController.RightHand)
                {
                    canActuate = true;
                }
            }
            else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
            {
                if ((int) controller.Characteristics == (int) HandController.LeftHand)
                {
                    canActuate = true;
                }
            }

            if (canActuate)
            {
                // StartCoroutine(Co_PrepActuation(controller));
                PrepActuation(controller);
            }
        }

        // private IEnumerator Co_HideArrow()
        // {
        //     Log($"{gameObject.name} {className} Co_HideArrow");

        //     yield return arrow?.Co_Hide();
        // }

        // private IEnumerator Co_ResetAcutation()
        // {
        //     Log($"{gameObject.name} {className} Co_ResetAcutation");

        //     if (!status.HasFlag(Status.Actuating))
        //     {
        //         // yield return Co_HideArrow();
        //         arrow.Hide();
        //     }

        //     RevokeStatus(Status.CanActuate);
        // }

        private void ResetAcutation()
        {
            // Log($"{gameObject.name} {className} Co_ResetAcutation");

            // if (!status.HasFlag(Status.Actuating))
            // {
            //     arrow.Hide();
            // }

            if (!IsHeld) arrow.Alpha = 0f;

            // actuationHandler.gameObject.SetActive(true);
            RevokeStatus(Status.CanActuate);
        }

        private void OnActuatorExit(GameObject trigger)
        {
            // Log($"{gameObject.name} {className} OnActuatorExit Collider: {trigger.name}");

            // StartCoroutine(Co_ResetAcutation());
            ResetAcutation();
        }

        private void OnActuatorEvent(TriggerEventType type, GameObject gameObject)
        {
            switch (type)
            {
                case TriggerEventType.OnTriggerEnter:
                    OnActuatorEnter(gameObject);
                    break;

                case TriggerEventType.OnTriggerExit:
                    OnActuatorExit(gameObject);
                    break;
            }
        }

        private void OnProximityHandler(float distance, GameObject gameObject)
        {
            // Debug.Log($"OnProximityHandler:Status Radius: {proximityHandler.Radius}");

            // Log($"{this.gameObject.name} {className} OnProximityHandler:Stats IsHeld: {IsHeld} OpposingCtrl: {opposingController} Distance: {distance} GameObject: {gameObject.name}");

            if ((!IsHeld) || status.HasFlag(Status.Actuating) || (!ReferenceEquals(gameObject, opposingController.gameObject))) return;

            // Log($"{this.gameObject.name} {className} OnProximityHandler:GameObject: {gameObject.name}");

            // if (!ReferenceEquals(gameObject, rightController.gameObject)) return;

            // Log($"{this.gameObject.name} {className} OnProximityHandler:Stats Distance: {distance}");

            arrow.Alpha = 1f - (distance / proximityHandler.Radius);
        }
    }
}