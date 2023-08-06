using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

public abstract class ControllerCanvasManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Manager")]
    [SerializeField] HandActionCanvasManager handStateCanvasManager;

    [Header("Components")]
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject grip;
    [SerializeField] GameObject thumbstick;
    [SerializeField] GameObject thumbstickClick;
    [SerializeField] GameObject thumbstickCursor;

    [Header("Optional Settings")]
    [SerializeField] bool enableThumbstickRaw;

    private Enum.ControllerEnums.Input lastGesture;

    void OnEnable()
    {
        HandController.InputChangeEventReceived += OnActuation;
        HandController.RawInputEventReceived += OnRawData;
        HandController.ActionEventReceived += OnActionEvent;
    }

    void OnDisable()
    {
        HandController.InputChangeEventReceived -= OnActuation;
        HandController.RawInputEventReceived -= OnRawData;
        HandController.ActionEventReceived -= OnActionEvent;
    }

    private void SetTriggerState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Trigger) && !trigger.activeSelf)
        {
            trigger.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Trigger) && trigger.activeSelf)
        {
            trigger.SetActive(false);
        }
    }

    private void SetGripState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Grip) && !grip.activeSelf)
        {
            grip.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Grip) && grip.activeSelf)
        {
            grip.SetActive(false);
        }
    }

    private void SetThumbstickState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Up) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Left) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Right) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Down) && !thumbstick.activeSelf)
        {
            thumbstick.SetActive(true);
        }

        if (!(actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Up) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Left) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Right) || actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Down)) && thumbstick.activeSelf)
        {
            thumbstick.SetActive(false);
        }
    }

    private void SetThumbstickClickState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Click) && !thumbstickClick.activeSelf)
        {
            thumbstickClick.SetActive(true);
        }

        if (!(actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Click) && thumbstickClick.activeSelf))
        {
            thumbstickClick.SetActive(false);
        }
    }

    public abstract void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics);

    public virtual void SetActuation(Enum.ControllerEnums.Input actuation)
    {
        Log($"{gameObject.name} {className} SetActuation:Actuation : {actuation}");

        lastGesture = actuation;

        SetTriggerState(actuation);
        SetGripState(actuation);
        SetThumbstickState(actuation);
        SetThumbstickClickState(actuation);
    }

    public abstract void OnRawData(HandController.RawInput rawData, InputDeviceCharacteristics characteristics);
    
    public void SetThumbstickRaw(Vector2 value)
    {
        Log($"{gameObject.name} {className} SetThumbstickRaw:Value : {value}");

        if (!enableThumbstickRaw) return;

        float x = value.x * 4f;
        float y = value.y * 4f;

        if (!thumbstickCursor.activeSelf)
        {
            thumbstickCursor.SetActive(true);
        }

        thumbstickCursor.transform.GetChild(0).localPosition = new Vector3(x, y, 0f);
    }

    public abstract void OnActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable);

    public void SetAction(Enum.HandEnums.Action action)
    {
        Log($"{gameObject.name} {className} SetAction Action: {action}");
        handStateCanvasManager.SetAction(action);
    }
}