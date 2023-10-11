// using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

namespace Interactables.Bow
{
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(Animator))]
    public class AltBowInteractableManager : FocusableInteractableManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] OnProximityHandler proximityHandler;
        [SerializeField] OnTriggerHandler actuationHandler;
        [SerializeField] LineRendererHelper lineRendererHelper;
        [SerializeField] Transform start;
        [SerializeField] Transform end;
        [SerializeField] Transform arrowAttachTransform;
        // [SerializeField] float pullRange = 0.25f;

        [Header("Prefabs")]
        [SerializeField] GameObject arrowPrefab;

        [Header("Config")]
        [SerializeField] int startingArrowCount = 5;
        [SerializeField] bool enableHaptics;
        [SerializeField] bool showLineRenderer;
        [SerializeField] InputAction fireAction;

        // [SerializeField] GameObject target;

        private GameObject bowParts;
        private Animator animator;
        private HandController leftController, rightController, opposingController, triggeredBy;
        // private InputDevice leftDevice, rightDevice;
        private Status status;
        // private int arrowCount;
        private Arrow arrow;
        private float pullValue;

        protected override void Awake()
        {
            base.Awake();

            bowParts = transform.GetChild(0).gameObject;
            animator = GetComponent<Animator>();
            status = Status.None;
            // arrowCount = startingArrowCount;
            // arrow.Hide();
            // arrow.Alpha = 0f;

            FabricateArrow();
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
            fireAction.Enable();
        }

        void OnDisable()
        {
            HandController.InputChangeEventReceived -= OnActuation;
            actuationHandler.EventReceived -= OnActuatorEvent;
            proximityHandler.EventReceived -= OnProximityHandler;
            fireAction.Disable();
        }

        private float CalculatePull()
        {
            // float distance = Vector3.Distance(start.position, triggeredBy.transform.position);
            // float pull = Mathf.Clamp01(distance / pullRange);

            // float pullRange = Vector3.Distance(start.position, end.position);
            // float distance = Vector3.Distance(start.position, triggeredBy.transform.position);
            // float pull = Mathf.Clamp01(distance / pullRange);
            
            Vector3 direction = end.position - start.position;
            float magnitude = direction.magnitude;
            direction.Normalize();

            Vector3 difference = triggeredBy.transform.position - start.position;
            return Mathf.Clamp01(Vector3.Dot(difference, direction) / magnitude);
        }

        // Update is called once per frame
        void Update()
        {
            // Log($"{className} Update Status: {status}");

            // actuationHandler.gameObject.SetActive(IsHeld);

            var enableTracking = IsHeld && status.HasFlag(Status.Actuating);

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
                // transform.LookAt(transform.position - (triggeredBy.transform.position - transform.position));
                
                // Quaternion rotation = transform.localRotation;
                // rotation.z = triggeredBy.transform.localRotation.z;
                // transform.localRotation = rotation;

                // Log($"Local Euler: {IsHeldBy.transform.localEulerAngles}");
                // Log($"Local Rotation: {IsHeldBy.transform.localRotation}");

                // Quaternion rotation = bowParts.transform.localRotation;
                // rotation.z = IsHeldBy.transform.localRotation.z;
                // bowParts.transform.localRotation = rotation;

                // Vector3 rotation = bowParts.transform.localEulerAngles;
                // rotation.z = IsHeldBy.transform.localEulerAngles.z;
                // bowParts.transform.localEulerAngles = rotation;

                pullValue = 1f;//CalculatePull();
                animator.SetFloat("Blend", pullValue);

                if (enableHaptics)
                {
                    triggeredBy.SetImpulse(0.1f);
                }
            }
            else
            {
                animator.SetFloat("Blend", 0f);

                if (IsHeld)
                {
                    transform.rotation = IsHeldBy.transform.rotation;
                }
            }

            lineRendererHelper.Active = enableTracking;

            // transform.LookAt(target.transform.position);
            // transform.LookAt(transform.position - (target.transform.position - transform.position));

            if (fireAction.triggered)
            {
                Fire();
            }
        }

        private void Fire()
        {
            var instance = Instantiate(arrowPrefab, arrowAttachTransform);
            arrow = instance.GetComponent<Arrow>();
            arrow.Fire(1f);
        }

        private void AddStatus(Status status) => this.status |= status;

        private void RevokeStatus(Status status) => this.status &= ~status;

        private void FabricateArrow()
        {
            // if (arrowCount == 0) return;

            var instance = Instantiate(arrowPrefab, arrowAttachTransform);
            arrow = instance.GetComponent<Arrow>();
            arrow.Alpha = 0f;

            // Log($"{className} {Time.time} FabricateArrow Instance ID: {instance.GetInstanceID()}");
        }

        private void TriggerArrow()
        {
            // Log($"{className} {Time.time} TriggerArrow PullValue: {pullValue}");

            arrow.Fire(1f/*pullValue*/);

            if (enableHaptics)
            {
                triggeredBy.SetImpulse(0.5f);
            }

            // --arrowCount;
            FabricateArrow();
        }

        // private IEnumerator Co_FireArrow(HandController controller)
        // {
        //     // Log($"{className} {Time.time} Co_FireArrow Controller: {controller.name}");

        //     yield return Co_HideArrow();
        //     RevokeStatus(Status.Actuating);
        //     controller.ShowModel();

        //     FireArrow();
        //     AddStatus(Status.Firing);
        // }

        private void ResetBowOrientation() => bowParts.transform.localEulerAngles = Vector3.zero;

        private void FireArrow(HandController controller)
        {
            // Log($"{Time.time} FireArrow Controller: {controller.gameObject.name}");

            // Log($"{className} {Time.time} Co_FireArrow Controller: {controller.name}");

            // arrow.Hide();
            // arrow.Alpha = 0f;
            RevokeStatus(Status.Actuating);
            controller.ShowModel();
            TriggerArrow();
            // AddStatus(Status.Firing);
            ResetBowOrientation();
        }

        private void OnTriggering(bool canActuate, HandController controller)
        {
            // Log($"{className} {Time.time} OnTriggering CanActuate: {canActuate} Controller: {controller.name}");

            if (canActuate)
            {
                AddStatus(Status.Actuating);
                arrow.Alpha = 1f;
                controller.HideModel();
            }
        }

        private void OnReleasing(bool actuating, HandController controller)
        {
            Log($"{className} {Time.time} OnReleasing Actuating: {actuating} Controller: {controller.name}");

            // if (actuating && pullValue > 0f)
            {
                // StartCoroutine(Co_FireArrow(controller));
                // FireArrow(controller);
                // RevokeStatus(Status.Actuating);
                // controller.ShowModel();
                Fire();
            }
        }

        private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        {
            // Log($"{className} {Time.time} OnActuation Controller: {controller.name} Actuation: {actuation}");

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
            // Log($"{className} {Time.time} OnHoldStatusChange IsHeld: {isHeld} IsHeldBy: {isHeldBy}");

            if (isHeld)
            {
                isHeldBy.HideModel();
                opposingController = GetOpposingController(isHeldBy);
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
        //     Log($"{className} {Time.time} Co_ShowArrow");

        //     yield return arrow?.Co_Show();
        // }

        // private IEnumerator Co_PrepActuation(HandController controller)
        // {
        //     Log($"{className} {Time.time} Co_PrepActuation Controller: {controller.name}");

        //     yield return Co_ShowArrow();
        //     AddStatus(Status.CanActuate);
        //     triggeredBy = controller;
        // }

        private void PrepActuation(HandController controller)
        {
            // Log($"{className} {Time.time} Co_PrepActuation Controller: {controller.name}");

            // actuationHandler.gameObject.SetActive(false);
            triggeredBy = controller;
            AddStatus(Status.CanActuate);
        }

        private void OnActuatorEnter(GameObject trigger)
        {
            // Log($"{className} {Time.time} OnActuatorEnter Collider: {trigger.name}");

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
        //     Log($"{className} {Time.time} Co_HideArrow");

        //     yield return arrow?.Co_Hide();
        // }

        // private IEnumerator Co_ResetAcutation()
        // {
        //     Log($"{className} {Time.time} Co_ResetAcutation");

        //     if (!status.HasFlag(Status.Actuating))
        //     {
        //         // yield return Co_HideArrow();
        //         arrow.Hide();
        //     }

        //     RevokeStatus(Status.CanActuate);
        // }

        private void ResetAcutation()
        {
            // Log($"{className} {Time.time} Co_ResetAcutation");

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
            // Log($"{className} {Time.time} OnActuatorExit Collider: {trigger.name}");

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
            // Debug.Log($"{className} {Time.time} OnProximityHandler:Status Radius: {proximityHandler.Radius}");

            // Log($"{this.gameObject.name} {className} OnProximityHandler:Stats IsHeld: {IsHeld} OpposingCtrl: {opposingController} Distance: {distance} GameObject: {gameObject.name}");

            if ((!IsHeld) || status.HasFlag(Status.Actuating) || (!ReferenceEquals(gameObject, opposingController.gameObject))) return;

            // Log($"{this.gameObject.name} {className} OnProximityHandler:GameObject: {gameObject.name}");

            // if (!ReferenceEquals(gameObject, rightController.gameObject)) return;

            // Log($"{this.gameObject.name} {className} OnProximityHandler:Stats Distance: {distance}");

            arrow.Alpha = 1f - (distance / proximityHandler.Radius);
        }
    }
}