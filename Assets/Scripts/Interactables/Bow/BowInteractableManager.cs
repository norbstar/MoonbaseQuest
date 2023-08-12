using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interactables.Bow
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class BowInteractableManager : FocusableInteractableManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] OnTriggerHandler actuationHandler;
        [SerializeField] LineRendererHelper lineRendererHelper;
        [SerializeField] Arrow arrow;

        // [SerializeField] GameObject target;

        private GameObject bowParts;
        private HandController leftController, rightController, triggeredBy;
        private Status status;

        protected override void Awake()
        {
            base.Awake();

            bowParts = transform.GetChild(0).gameObject;
            status = Status.None;
            arrow.Hide();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (TryGet.TryGetControllers(out List<HandController> controllers))
            {
                foreach (HandController controller in controllers)
                {
                    if ((int) controller.Characteristics == (int) HandController.LeftHand)
                    {
                        leftController = controller;
                    }
                    else if ((int) controller.Characteristics == (int) HandController.RightHand)
                    {
                        rightController = controller;
                    }
                }
            }
        }

        void OnEnable()
        {
            HandController.InputChangeEventReceived += OnActuation;
            actuationHandler.EventReceived += OnActuatorEvent;
        }

        void OnDisable()
        {
            HandController.InputChangeEventReceived -= OnActuation;
            actuationHandler.EventReceived -= OnActuatorEvent;
        }

        // Update is called once per frame
        void Update()
        {
            Log($"{gameObject.name} {className} Update Status: {status}");

            actuationHandler.gameObject.SetActive(IsHeld);

            var enableTracking = status.HasFlag(Status.IsHeld) && status.HasFlag(Status.Actuating);

            if (enableTracking)
            {
                lineRendererHelper.ConfigLine(transform.position, triggeredBy.transform.position);

                // var lookPos = triggeredBy.transform.position - transform.position;
                // var rotation = Quaternion.LookRotation(lookPos);
                // transform.rotation = rotation;

                // transform.LookAt(triggeredBy.transform.position);
                // transform.LookAt(transform.position - (triggeredBy.transform.position - transform.position));
                
                // Quaternion rotation = transform.localRotation;
                // rotation.z = triggeredBy.transform.localRotation.z;
                // transform.localRotation = rotation;

                Log($"Local Euler: {IsHeldBy.transform.localEulerAngles}");
                Log($"Local Rotation: {IsHeldBy.transform.localRotation}");

                // Quaternion rotation = bowParts.transform.localRotation;
                // rotation.z = IsHeldBy.transform.localRotation.z;
                // bowParts.transform.localRotation = rotation;
            }
            else if (status.HasFlag(Status.IsHeld))
            {
                transform.rotation = IsHeldBy.transform.rotation;
            }

            lineRendererHelper.Active = enableTracking;

            // transform.LookAt(target.transform.position);
            // transform.LookAt(transform.position - (target.transform.position - transform.position));
        }

        private void AddStatus(Status status) => this.status |= status;

        private void RevokeStatus(Status status) => this.status &= ~status;

        private void FireArrow() { }

        private IEnumerator Co_FireArrow(HandController controller)
        {
            // Log($"{gameObject.name} {className} Co_FireArrow Controller: {controller.name}");

            yield return Co_HideArrow();
            RevokeStatus(Status.Actuating);
            controller.ShowModel();

            FireArrow();
            AddStatus(Status.Firing);
        }

        private void OnTriggering(bool canActuate, HandController controller)
        {
            // Log($"{gameObject.name} {className} OnTriggering CanActuate: {canActuate} Controller: {controller.name}");

            if (canActuate)
            {
                AddStatus(Status.Actuating);
                controller.HideModel();
            }
        }

        private void OnReleasing(bool actuating, HandController controller)
        {
            // Log($"{gameObject.name} {className} OnReleasing Actuating: {actuating} Controller: {controller.name}");

            if (actuating)
            {
                StartCoroutine(Co_FireArrow(controller));
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

        public override void OnHoldStatusChange(bool isHeld, HandController isHeldBy)
        {
            Log($"{gameObject.name} {className} OnHoldStatusChange IsHeld: {isHeld} IsHeldBy: {isHeldBy}");

            if (isHeld)
            {
                AddStatus(Status.IsHeld);
            }
            else
            {
                if (arrow.IsVisible)
                {
                    StartCoroutine(Co_HideArrow());
                }

                status = Status.None;
            }
        }

        private IEnumerator Co_ShowArrow()
        {
            Log($"{gameObject.name} {className} Co_ShowArrow");

            yield return arrow?.Co_Show();
        }

        private IEnumerator Co_PrepActuation(HandController controller)
        {
            Log($"{gameObject.name} {className} Co_PrepActuation Controller: {controller.name}");

            yield return Co_ShowArrow();
            AddStatus(Status.CanActuate);
            triggeredBy = controller;
        }

        private void OnActuatorEnter(GameObject trigger)
        {
            Log($"{gameObject.name} {className} OnActuatorEnter Collider: {trigger.name}");

            if (!status.HasFlag(Status.IsHeld)) return;

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
                StartCoroutine(Co_PrepActuation(controller));
            }
        }

        private IEnumerator Co_HideArrow()
        {
            Log($"{gameObject.name} {className} Co_HideArrow");

            yield return arrow?.Co_Hide();
        }

        private IEnumerator Co_ResetAcutation()
        {
            Log($"{gameObject.name} {className} Co_ResetAcutation");

            if (!status.HasFlag(Status.Actuating))
            {
                yield return Co_HideArrow();
            }

            RevokeStatus(Status.CanActuate);
        }

        private void OnActuatorExit(GameObject trigger)
        {
            Log($"{gameObject.name} {className} OnActuatorExit Collider: {trigger.name}");

            StartCoroutine(Co_ResetAcutation());
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
    }
}