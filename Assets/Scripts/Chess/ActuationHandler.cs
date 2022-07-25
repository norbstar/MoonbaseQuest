using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class ActuationHandler : MonoBehaviour
{
    public enum Intent
    {
        ToggleOculusMenu
    }

    public delegate void IntentEvent(Intent intent);
    public event IntentEvent IntentEventReceived;

    void OnEnable() => HandController.ActuationEventReceived += OnActuation;

    void OnDisable() => HandController.ActuationEventReceived -= OnActuation;

    public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if (actuation.HasFlag(Actuation.Menu_Oculus))
        {
            IntentEventReceived?.Invoke(Intent.ToggleOculusMenu);
        }
    }
}