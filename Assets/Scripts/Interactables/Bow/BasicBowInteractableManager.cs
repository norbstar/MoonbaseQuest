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
    public class BasicBowInteractableManager : FocusableInteractableManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] Transform start;
        [SerializeField] Transform end;
        [SerializeField] Transform arrowAttachTransform;

        [Header("Prefabs")]
        [SerializeField] GameObject arrowPrefab;

        [Header("Config")]
        [SerializeField] InputAction fireAction;

        private GameObject bowParts;
        private Animator animator;
        private HandController leftController, rightController, opposingController;
        private bool isActuating, wasTriggering;
        private Arrow arrow;
        private float pullValue;

        public bool IsActuating { get { return isActuating; } }

        protected override void Awake()
        {
            base.Awake();

            bowParts = transform.GetChild(0).gameObject;
            animator = GetComponent<Animator>();

            InstantiateArrow();
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
            fireAction.Enable();
        }

        void OnDisable()
        {
            HandController.InputChangeEventReceived -= OnActuation;
            fireAction.Disable();
        }

        private float CalculatePull()
        {
            float pullRange = Vector3.Distance(start.position, end.position);
            float distance = Vector3.Distance(start.position, opposingController.transform.position);
            return Mathf.Clamp01(distance / pullRange);

            // Vector3 direction = end.position - start.position;
            // float magnitude = direction.magnitude;
            // direction.Normalize();

            // Vector3 difference = opposingController.transform.position - start.position;
            // return Mathf.Clamp01(Vector3.Dot(difference, direction) / magnitude);
        }

        // Update is called once per frame
        void Update()
        {
            // Log($"{className} Update Status: {status}");

            if (IsHeld && IsActuating)
            {
                pullValue = 1f;//CalculatePull();
                animator.SetFloat("Blend", pullValue);
            }

            // if (fireAction.triggered)
            // {
            //     pullValue = 1f;
            //     FireArrow(rightController);
            //     InstantiateArrow();
            // }
        }

        private void InstantiateArrow()
        {
            // Log($"{className} {Time.time} InstantiateArrow");

            var instance = Instantiate(arrowPrefab, arrowAttachTransform);
            arrow = instance.GetComponent<Arrow>();
        }

        private void TriggerArrow() => arrow.Fire(pullValue);

        // private void ResetBowOrientation() => bowParts.transform.localEulerAngles = Vector3.zero;

        private void FireArrow(HandController controller)
        {
            // Log($"{Time.time} FireArrow Controller: {controller.gameObject.name}");

            TriggerArrow();
            // ResetBowOrientation();
        }

        private void OnTriggering(HandController controller)
        {
            Log($"{className} {Time.time} OnTriggering Controller: {controller.name}");

            // pullValue = 1f;
            // FireArrow(controller);
            // InstantiateArrow();

            isActuating = true;
        }

        private void OnReleasing(HandController controller)
        {
            Log($"{className} {Time.time} OnReleasing Controller: {controller.name}");

            // FireArrow(controller);
            // InstantiateArrow();

            animator.SetFloat("Blend", 0f);
            isActuating = false;
            
            if (pullValue > 0f)
            {
                FireArrow(controller);
                InstantiateArrow();
            }
        }

        // private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        // {
        //     if (!IsHeld) return;

        //     if ((int) controller.Characteristics == (int) HandController.RightHand)
        //     {
        //         var isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

        //         if (isTriggering)
        //         {
        //             pullValue = 1f;
        //             FireArrow(rightController);
        //             InstantiateArrow();
        //         }
        //     }

        // }

        private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        {
            // Log($"{className} {Time.time} OnActuation Controller: {controller.name} Actuation: {actuation}");

            if (!IsHeld) return;

            var isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

            if (isTriggering)
            {
                OnTriggering(rightController);
            }
            else if (wasTriggering)
            {
                OnReleasing(rightController);
            }

            wasTriggering = isTriggering;
        }

        // private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        // {
        //     // Log($"{className} {Time.time} OnActuation Controller: {controller.name} Actuation: {actuation}");

        //     if (!IsHeld) return;

        //     var isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

        //     if (isTriggering)
        //     {
        //         OnTriggering(opposingController);
        //     }
        //     else if (wasTriggering)
        //     {
        //         OnReleasing(opposingController);
        //     }

        //     wasTriggering = isTriggering;
        // }

        private HandController GetOpposingController(HandController controller) => ((int) controller.Characteristics == (int) HandController.LeftHand) ? rightController : leftController;

        public override void OnHoldStatusChange(bool isHeld, HandController isHeldBy)
        {
            // Log($"{className} {Time.time} OnHoldStatusChange IsHeld: {isHeld} IsHeldBy: {isHeldBy}");

            if (isHeld)
            {
                opposingController = GetOpposingController(isHeldBy);
            }
            else
            {
                // bowParts.transform.localEulerAngles = Vector3.zero;
                animator.SetFloat("Blend", 0f);
                opposingController = null;
            }
        }
    }
}