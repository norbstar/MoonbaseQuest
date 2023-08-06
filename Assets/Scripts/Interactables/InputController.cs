using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class InputController : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
    [SerializeField] AnalyticsTerminalCanvas analyticsTerminalCanvas;

    // public InteractableManager targetInteractable;

    // [Header("Prompts")]
    // public AudioClip leftHandTrigger;
    // public AudioClip leftHandGrip;
    // public AudioClip rightHandTrigger;
    // public AudioClip rightHandGrip;

    private bool isHoldingBowLeftHand, isHoldingBowRightHand;
    
    void OnEnable()
    {
        // HandController.InputChangeEventReceived += OnInputChangeEvent;
        // HandController.HoveringEventReceived += OnHoveringEvent;
        // HandController.HoldingEventReceived += OnHoldingEvent;
        // HandController.RawInputEventReceived += OnRawInputEvent;
        // HandController.ActionEventReceived += OnActionEvent;
        HandController.InputChangeEventReceived += OnActuation;
    }

    void OnDisable()
    {
        // andController.InputChangeEventReceived -= OnInputChangeEvent;
        // HandController.HoveringEventReceived -= OnHoveringEvent;
        // HandController.HoldingEventReceived -= OnHoldingEvent;
        // HandController.RawInputEventReceived -= OnRawInputEvent;
        // HandController.ActionEventReceived -= OnActionEvent;
        HandController.InputChangeEventReceived -= OnActuation;
    }

    public void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand && actuation == Enum.ControllerEnums.Input.Menu_Oculus)
        {
            analyticsTerminalCanvas.ClearLog();
        }
    }

    private void OnHoveringEvent(bool isHovering, IInteractable interactable, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"OnHoveringEvent Left Hand: {isHovering}");
        }
        else if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"OnHoveringEvent Right Hand: {isHovering}");
        }
    }

    private void OnHoldingEvent(bool isHolding, IInteractable interactable, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"OnHoldingEvent Left Hand: {isHolding}");
        }
        else if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"OnHoldingEvent Right Hand: {isHolding}");
        }
    }

    // private void OnLeftHandActuation(HandController controller, Actuation actuation, InputDeviceCharacteristics characteristics)
    // {
    //     Debug.Log($"OnActuation Left Hand: {actuation}");

    //     switch (actuation)
    //     {
    //         case Actuation.Grip:
    //             AudioSource.PlayClipAtPoint(leftHandGrip, Vector3.zero, 1.0f);
    //             break;

    //         case Actuation.Trigger:
    //             AudioSource.PlayClipAtPoint(leftHandTrigger, Vector3.zero, 1.0f);
    //             break;
    //     }
    // }

    // private void OnRightHandActuation(HandController controller, Actuation actuation, InputDeviceCharacteristics characteristics)
    // {
    //     Debug.Log($"OnActuation Right Hand : {actuation}");

    //     switch (actuation)
    //     {
    //         case Actuation.Grip:
    //             AudioSource.PlayClipAtPoint(rightHandGrip, Vector3.zero, 1.0f);
    //             break;

    //         case Actuation.Trigger:
    //             AudioSource.PlayClipAtPoint(rightHandTrigger, Vector3.zero, 1.0f);
    //             break;
    //     }
    // }

    private void OnInputChangeEvent(HandController controller, Enum.ControllerEnums.Input input, InputDeviceCharacteristics characteristics)
    {
        Log($"{gameObject.name} {className} OnInputChangeEvent");

        // if ((int) characteristics == (int) HandController.LeftHand)
        // {
        //     OnLeftHandActuation(controller, actuation, characteristics);
        // }
        // else if ((int) characteristics == (int) HandController.RightHand)
        // {
        //     OnRightHandActuation(controller, actuation, characteristics);
        // }
    }

    private void OnRawInputEvent(HandController.RawInput rawInput, InputDeviceCharacteristics characteristics)
    {
        // bool isLeftHand = ((int) characteristics == (int) HandController.LeftHand);
        // bool isRightHand = ((int) characteristics == (int) HandController.RightHand);

        // if ((int) characteristics == (int) HandController.LeftHand)
        // {
        //     Log($"{gameObject.name} {className} OnRawData LeftHand TriggerValue: {rawData.triggerValue} GripValue: {rawData.gripValue}");
        // }
        // else if ((int) characteristics == (int) HandController.RightHand)
        // {
        //     Log($"{gameObject.name} {className} OnRawData RightHand TriggerValue: {rawData.triggerValue} GripValue: {rawData.gripValue}");
        // }
        
        // if (targetInteractable.IsHeld)
        // {
        //     if ((int) targetInteractable.IsHeldBy.Characteristics == (int) HandController.LeftHand)
        //     {
        //         Log($"{gameObject.name} {className} OnRawInputEvent:IsHeld: True, IsHeldBy: Left Hand");
        //     }
        //     else if ((int) targetInteractable.IsHeldBy.Characteristics == (int) HandController.RightHand)
        //     {
        //         Log($"{gameObject.name} {className} OnRawInputEvent:IsHeld: True, IsHeldBy: Right Hand");
        //     }
        // }
        // else
        // {
        //     Log($"{gameObject.name} {className} OnRawInputEvent:IsHeld: False, IsHeldBy: None");
        // }
    }

    private bool IsHovering(Enum.HandEnums.Action action) => (action & Enum.HandEnums.Action.Hovering) == Enum.HandEnums.Action.Hovering;

    private bool IsHolding(Enum.HandEnums.Action action) => (action & Enum.HandEnums.Action.Holding) == Enum.HandEnums.Action.Holding;

    private void OnActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{gameObject.name} {className} OnActionEvent:Left Hand Interactable: {interactable.GetGameObject().name} Hovering: {IsHovering(action)} Holding: {IsHolding(action)}");
        }
        else if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{gameObject.name} {className} OnActionEvent:Right Hand Interactable: {interactable.GetGameObject().name} Hovering: {IsHovering(action)} Holding: {IsHolding(action)}");
        }

        // TODO
    }
}
