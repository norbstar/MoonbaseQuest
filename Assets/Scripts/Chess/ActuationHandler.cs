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

    void OnEnable() => HandController.InputChangeEventReceived += OnActuation;

    void OnDisable() => HandController.InputChangeEventReceived -= OnActuation;

    public void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus))
        {
            IntentEventReceived?.Invoke(Intent.ToggleOculusMenu);
        }
    }
}