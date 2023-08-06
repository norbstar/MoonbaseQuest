using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

public class RightControllerCanvasManager : ControllerCanvasManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Other Components")]
    [SerializeField] GameObject oculusButton;
    [SerializeField] GameObject aButton;
    [SerializeField] GameObject bButton;

    private void SetOculusButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus) && !oculusButton.activeSelf)
        {
            oculusButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus) && oculusButton.activeSelf)
        {
            oculusButton.SetActive(false);
        }
    }

    private void SetAButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX) && !aButton.activeSelf)
        {
            aButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX) && aButton.activeSelf)
        {
            aButton.SetActive(false);
        }
    }
    
    private void SetBButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_BY) && !bButton.activeSelf)
        {
            bButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_BY) && bButton.activeSelf)
        {
            bButton.SetActive(false);
        }
    }

    public override void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(Enum.ControllerEnums.Input actuation)
    {
        base.SetActuation(actuation);

        SetOculusButtonState(actuation);
        SetAButtonState(actuation);
        SetBButtonState(actuation);
    }

    public override void OnRawData(HandController.RawInput rawData, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{gameObject.name} {className} OnRawData:Value : {rawData.thumbstickValue}");
            SetThumbstickRaw(rawData.thumbstickValue);
        }
    }

    public override void OnActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable)
    {
        Log($"{gameObject.name} {className} OnActionEvent Action: {action} Characteristics: {characteristics}");
        
        if ((int) characteristics == (int) HandController.RightHand)
        {
            SetAction(action);
        }
    }
}