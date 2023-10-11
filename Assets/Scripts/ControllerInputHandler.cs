using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ControllerInputHandler : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] UnityEvent onMenuButton;

    void OnEnable() => HandController.InputChangeEventReceived += OnActuation;

    void OnDisable() => HandController.InputChangeEventReceived -= OnActuation;

    private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            if (actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus))
            {
                onMenuButton.Invoke();
            }
        }
    }
}
