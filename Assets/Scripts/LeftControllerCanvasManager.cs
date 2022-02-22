using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;
using static Enum.HandEnums;

public class LeftControllerCanvasManager : ControllerCanvasManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Other Components")]
    [SerializeField] GameObject menuButton;
    [SerializeField] GameObject xButton;
    [SerializeField] GameObject yButton;

    private void SetMenuButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Menu_Oculus) && !menuButton.activeSelf)
        {
            menuButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Menu_Oculus) && menuButton.activeSelf)
        {
            menuButton.SetActive(false);
        }
    }

    private void SetXButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Button_AX) && !xButton.activeSelf)
        {
            xButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Button_AX) && xButton.activeSelf)
        {
            xButton.SetActive(false);
        }
    }
    
    private void SetYButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Button_BY) && !yButton.activeSelf)
        {
            yButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Button_BY) && yButton.activeSelf)
        {
            yButton.SetActive(false);
        }
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(Actuation actuation)
    {
        base.SetActuation(actuation);

        SetMenuButtonState(actuation);
        SetXButtonState(actuation);
        SetYButtonState(actuation);
    }

    public override void OnRawData(HandController.RawData rawData, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnThumbstickRaw:Value : {rawData.thumbstickValue}");
            SetThumbstickRaw(rawData.thumbstickValue);
        }
    }

    public override void OnState(Enum.HandEnums.State state, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnState:State : {state}");
            SetState(state);
        }
    }
}