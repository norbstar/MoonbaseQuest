using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

public abstract class ControllerCanvasManager : MonoBehaviour
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Manager")]
    [SerializeField] HandStateCanvasManager handGestureCanvasManager;

    [Header("Components")]
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject grip;
    [SerializeField] GameObject thumbstick;
    [SerializeField] GameObject thumbstickClick;
    [SerializeField] GameObject thumbstickCursor;

    [Header("Optional Settings")]
    [SerializeField] bool enableThumbstickRaw;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private HandController.Actuation lastGesture;

    void OnEnable()
    {
        HandController.ActuationEventReceived += OnActuation;
        HandController.RawDataEventReceived += OnRawData;
        HandController.StateEventReceived += OnState;
    }

    void OnDisable()
    {
        HandController.ActuationEventReceived -= OnActuation;
        HandController.RawDataEventReceived -= OnRawData;
        HandController.StateEventReceived -= OnState;
    }

    private void SetTriggerState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Trigger) && !trigger.activeSelf)
        {
            trigger.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Trigger) && trigger.activeSelf)
        {
            trigger.SetActive(false);
        }
    }

    private void SetGripState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Grip) && !grip.activeSelf)
        {
            grip.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Grip) && grip.activeSelf)
        {
            grip.SetActive(false);
        }
    }

    private void SetThumbstickState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Thumbstick_Up) || actuation.HasFlag(HandController.Actuation.Thumbstick_Left) || actuation.HasFlag(HandController.Actuation.Thumbstick_Right) || actuation.HasFlag(HandController.Actuation.Thumbstick_Down) && !thumbstick.activeSelf)
        {
            thumbstick.SetActive(true);
        }

        if (!(actuation.HasFlag(HandController.Actuation.Thumbstick_Up) || actuation.HasFlag(HandController.Actuation.Thumbstick_Left) || actuation.HasFlag(HandController.Actuation.Thumbstick_Right) || actuation.HasFlag(HandController.Actuation.Thumbstick_Down)) && thumbstick.activeSelf)
        {
            thumbstick.SetActive(false);
        }
    }

    private void SetThumbstickClickState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Thumbstick_Click) && !thumbstickClick.activeSelf)
        {
            thumbstickClick.SetActive(true);
        }

        if (!(actuation.HasFlag(HandController.Actuation.Thumbstick_Click) && thumbstickClick.activeSelf))
        {
            thumbstickClick.SetActive(false);
        }
    }

    public abstract void OnActuation(HandController.Actuation actuation, InputDeviceCharacteristics characteristics);

    public virtual void SetActuation(HandController.Actuation actuation)
    {
        Log($"{Time.time} {gameObject.name} {className} SetActuation:Actuation : {actuation}");

        lastGesture = actuation;

        SetTriggerState(actuation);
        SetGripState(actuation);
        SetThumbstickState(actuation);
        SetThumbstickClickState(actuation);
    }

    public abstract void OnRawData(HandController.RawData rawData, InputDeviceCharacteristics characteristics);
    
    public void SetThumbstickRaw(Vector2 value)
    {
        Log($"{Time.time} {gameObject.name} {className} SetThumbstickRaw:Value : {value}");

        if (!enableThumbstickRaw) return;

        float x = value.x * 4f;
        float y = value.y * 4f;

        if (!thumbstickCursor.activeSelf)
        {
            thumbstickCursor.SetActive(true);
        }

        thumbstickCursor.transform.GetChild(0).localPosition = new Vector3(x, y, 0f);
    }

    public abstract void OnState(HandStateCanvasManager.State state, InputDeviceCharacteristics characteristics);

    public void SetState(HandStateCanvasManager.State state)
    {
        Log($"{Time.time} {gameObject.name} {className} SetState:State : {state}");

        handGestureCanvasManager.SetState(state);
    }

    protected void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}