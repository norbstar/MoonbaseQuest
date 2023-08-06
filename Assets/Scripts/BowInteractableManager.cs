using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class BowInteractableManager : FocusableInteractableManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] MeshRenderer actuationZone;
    [SerializeField] OnPrimitiveTriggerHandler actuatorHandler;
    [SerializeField] Arrow mockArrow;

    public enum Stage
    {
        Default,
        Held,
        Actuated,
        Prepping,
        Firing
    }

    // [Header("Assets")]
    // [SerializeField] GameObject arrowPrefab = null;

    private HandController leftController, rightController, triggerController;
    private bool canTrigger, isTriggering;

    [Header("Components")]
    [SerializeField] LineRenderer line;

    private const float lineWidth = 0.01f;
    
    // Start is called before the first frame update
    void Start()
    {
        List<HandController> controllers;
        
        if (TryGet.TryGetControllers(out controllers))
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

        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;
    }

    void OnEnable()
    {
        HandController.InputChangeEventReceived += OnActuation;
        HandController.ActionEventReceived += OnActionEvent;
        IsHeldStatusChangeEventReceived += OnIsHeldStatusChange;
        actuatorHandler.EventReceived += OnActuatorEvent;
    }

    void OnDisable()
    {
        HandController.InputChangeEventReceived -= OnActuation;
        HandController.ActionEventReceived -= OnActionEvent;
        IsHeldStatusChangeEventReceived -= OnIsHeldStatusChange;
        actuatorHandler.EventReceived -= OnActuatorEvent;
    }

    // Update is called once per frame
    void Update()
    {
        // if (IsHeld)
        // {
        //     if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
        //     {
        //         Log($"{gameObject.name} {className} Update:IsHeld: True, IsHeldBy: Left Hand, Right Hand Position: {rightController.transform.position}");
        //     }
        //     else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
        //     {
        //         Log($"{gameObject.name} {className} Update:IsHeld: True, IsHeldBy: Right Hand, Left Hand Position: {leftController.transform.position}");
        //     }
        // }
        // else
        // {
        //     Log($"{gameObject.name} {className} Update:IsHeld: False, IsHeldBy: None");
        // }

        actuationZone.gameObject.SetActive(IsHeld);

        if (isTriggering && (triggerController != null))
        {
            // var direction =  transform.position - triggerHand.position;
            // Debug.DrawRay(triggerHand.position, direction, Color.green);
            line.SetPositions(new Vector3[] { transform.position, triggerController.transform.position });
        }

        line.gameObject.SetActive(isTriggering);

        // line.SetPositions(new Vector3[] { Vector3.zero, transform.position });
    }

    // private bool IsHovering(Enum.HandEnums.Action action) => (action & Enum.HandEnums.Action.Hovering) == Enum.HandEnums.Action.Hovering;

    // private bool IsHolding(Enum.HandEnums.Action action) => (action & Enum.HandEnums.Action.Holding) == Enum.HandEnums.Action.Holding;

    private void OnActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable)
    {
        // Log($"{gameObject.name} {className} OnActionEvent Interactable: {interactable.GetGameObject().name} Hovering: {IsHovering(action)} Holding: {IsHolding(action)}");

        // if (!IsHeld) return;

        // if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
        // {
        //     if ((int) characteristics == (int) HandController.RightHand)
        //     {
        //         Log($"{gameObject.name} {className} OnActionEvent Right Hand Action: {action}");
        //     }
        // }
        // else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
        // {
        //     if ((int) characteristics == (int) HandController.LeftHand)
        //     {
        //         Log($"{gameObject.name} {className} OnActionEvent Left Hand Action: {action}");
        //     }
        // }
    }

    private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        // if (!canTrigger) return;

        // if (actuation.HasFlag(Enum.ControllerEnums.Input.Trigger))
        // {
        //     if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
        //     {
        //         if ((int) characteristics == (int) HandController.RightHand)
        //         {
        //             rightController.HideModel();
        //             Log($"OnActuation Right Hand: {actuation}");
        //         }
        //     }
        //     else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
        //     {
        //         if ((int) characteristics == (int) HandController.LeftHand)
        //         {
        //             leftController.HideModel();
        //             Log($"OnActuation Left Hand : {actuation}");
        //         }
        //     }
        // }

        if (!IsHeld) return;

        if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
        {
            if ((int) characteristics == (int) HandController.RightHand)
            {
                // Log($"OnActuation Right Hand: {actuation}");
                isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

                if (canTrigger && isTriggering)
                {
                    rightController.HideModel();
                }
                else
                {
                    rightController.ShowModel();
                }
            }
        }
        else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
        {
            if ((int) characteristics == (int) HandController.LeftHand)
            {
                // Log($"OnActuation Left Hand: {actuation}");              
                isTriggering = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);

                if (canTrigger && isTriggering)
                {
                    leftController.HideModel();
                }
                else
                {
                    leftController.ShowModel();
                }
            }
        }
    }

    private void OnIsHeldStatusChange(bool isHeld, HandController isHeldBy)
    {
        if (!isHeld)
        {
            triggerController = null;
            isTriggering = false;
        }
    }

    private IEnumerator Co_Show(HandController triggerController)
    {
        yield return mockArrow?.Co_Show();
        this.triggerController = triggerController;
        canTrigger = true;
    }

    private void OnActuatorEnter(GameObject trigger)
    {
        Log($"{gameObject.name} {className} OnActuatorEnter Collider: {trigger.name}");

        if (!IsHeld) return;

        bool revealArrow = false;
        HandController triggerController = null;

        if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
        {
            // Log($"Left Hand Trigger: {trigger.name}");

            if ((int) trigger.GetComponent<HandController>().Characteristics == (int) HandController.RightHand)
            {
                triggerController = rightController;
                revealArrow = true;
            }
        }
        else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
        {
            // Log($"Right Hand Trigger: {trigger.name}");

            if ((int) trigger.GetComponent<HandController>().Characteristics == (int) HandController.LeftHand)
            {
                triggerController = leftController;
                revealArrow = true;
            }
        }

        if (revealArrow)
        {
            // if (arrow == null)
            // {
            //     var instance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            //     instance.transform.SetParent(transform);
            //     instance.transform.localEulerAngles = transform.forward;

            //     Vector3 localPosition = instance.transform.localPosition;
            //     localPosition.z = transform.localPosition.z + 0.25f;
            //     instance.transform.localPosition = localPosition;

            //     arrow = instance.GetComponent<Arrow>();
            // }

            StartCoroutine(Co_Show(triggerController));
        }
    }

    private IEnumerator Co_HideAndDestroy()
    {
        yield return mockArrow?.Co_Hide();
        triggerController = null;
        canTrigger = false;
    }

    private void OnActuatorExit(GameObject trigger)
    {
        Log($"{gameObject.name} {className} OnActuatorExit Collider: {trigger.name}");
        StartCoroutine(Co_HideAndDestroy());
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

    // Start is called before the first frame update
    // public void OnTriggerEnter(Collider collider)
    // {
    //     Log($"{gameObject.name} {className} OnTriggerEnter Collider: {collider.gameObject.name}");

    //     if (!IsHeld) return;

    //     var trigger = collider.gameObject;

    //     bool revealArrow = false;

    //     if ((int) IsHeldBy.Characteristics == (int) HandController.LeftHand)
    //     {
    //         // Log($"Left Hand Trigger: {trigger.name}");

    //         if ((int) trigger.GetComponent<HandController>().Characteristics == (int) HandController.RightHand)
    //         {
    //             revealArrow = true;
    //         }
    //     }
    //     else if ((int) IsHeldBy.Characteristics == (int) HandController.RightHand)
    //     {
    //         // Log($"Right Hand Trigger: {trigger.name}");

    //         if ((int) trigger.GetComponent<HandController>().Characteristics == (int) HandController.LeftHand)
    //         {
    //             revealArrow = true;
    //         }
    //     }

    //     if (revealArrow)
    //     {
    //         if (arrow == null)
    //         {
    //             var instance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
    //             instance.transform.SetParent(transform);
    //             instance.transform.localEulerAngles = transform.forward;

    //             Vector3 localPosition = instance.transform.localPosition;
    //             localPosition.z = transform.localPosition.z + 0.25f;
    //             instance.transform.localPosition = localPosition;

    //             arrow = instance.GetComponent<Arrow>();
    //         }

    //         StartCoroutine(Co_Show());
    //     }
    // }

    // public void OnTriggerExit(Collider collider)
    // {
    //     Log($"{gameObject.name} {className} OnTriggerExit Collider: {collider.gameObject.name}");
    //     StartCoroutine(Co_HideAndDestroy());
    // }
}