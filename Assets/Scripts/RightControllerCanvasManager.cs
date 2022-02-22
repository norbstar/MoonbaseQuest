using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;
using static Enum.HandEnums;

public class RightControllerCanvasManager : ControllerCanvasManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Other Components")]
    [SerializeField] GameObject oculusButton;
    [SerializeField] GameObject aButton;
    [SerializeField] GameObject bButton;

    private void SetOculusButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Menu_Oculus) && !oculusButton.activeSelf)
        {
            oculusButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Menu_Oculus) && oculusButton.activeSelf)
        {
            oculusButton.SetActive(false);
        }
    }

    private void SetAButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Button_AX) && !aButton.activeSelf)
        {
            aButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Button_AX) && aButton.activeSelf)
        {
            aButton.SetActive(false);
        }
    }
    
    private void SetBButtonState(Actuation actuation)
    {
        if (actuation.HasFlag(Actuation.Button_BY) && !bButton.activeSelf)
        {
            bButton.SetActive(true);
        }

        if (!actuation.HasFlag(Actuation.Button_BY) && bButton.activeSelf)
        {
            bButton.SetActive(false);
        }
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(Actuation actuation)
    {
        base.SetActuation(actuation);

        SetOculusButtonState(actuation);
        SetAButtonState(actuation);
        SetBButtonState(actuation);
    }

    public override void OnRawData(HandController.RawData rawData, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnThumbstickRaw:Value : {rawData.thumbstickValue}");
            SetThumbstickRaw(rawData.thumbstickValue);
        }
    }

    public override void OnState(Enum.HandEnums.State state, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnState:State : {state}");
            SetState(state);
        }
    }
}