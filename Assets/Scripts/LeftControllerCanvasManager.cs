using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

public class LeftControllerCanvasManager : ControllerCanvasManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Other Components")]
    [SerializeField] GameObject menuButton;
    [SerializeField] GameObject xButton;
    [SerializeField] GameObject yButton;

    private void SetMenuButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus) && !menuButton.activeSelf)
        {
            menuButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus) && menuButton.activeSelf)
        {
            menuButton.SetActive(false);
        }
    }

    private void SetXButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX) && !xButton.activeSelf)
        {
            xButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX) && xButton.activeSelf)
        {
            xButton.SetActive(false);
        }
    }
    
    private void SetYButtonState(Enum.ControllerEnums.Input actuation)
    {
        if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_BY) && !yButton.activeSelf)
        {
            yButton.SetActive(true);
        }

        if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_BY) && yButton.activeSelf)
        {
            yButton.SetActive(false);
        }
    }

    public override void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(Enum.ControllerEnums.Input actuation)
    {
        base.SetActuation(actuation);

        SetMenuButtonState(actuation);
        SetXButtonState(actuation);
        SetYButtonState(actuation);
    }

    public override void OnRawData(HandController.RawInput rawData, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{gameObject.name} {className} OnRawData Value: {rawData.thumbstickValue}");
            SetThumbstickRaw(rawData.thumbstickValue);
        }
    }

    public override void OnActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable)
    {
        Log($"{gameObject.name} {className} OnActionEvent Action: {action} Characteristics: {characteristics}");
        
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            SetAction(action);
        }
    }
}