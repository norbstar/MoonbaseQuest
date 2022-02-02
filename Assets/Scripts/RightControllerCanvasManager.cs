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

    private void SetOculusButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Menu_Oculus) && !oculusButton.activeSelf)
        {
            oculusButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Menu_Oculus) && oculusButton.activeSelf)
        {
            oculusButton.SetActive(false);
        }
    }

    private void SetAButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Button_AX) && !aButton.activeSelf)
        {
            aButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Button_AX) && aButton.activeSelf)
        {
            aButton.SetActive(false);
        }
    }
    
    private void SetBButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Button_BY) && !bButton.activeSelf)
        {
            bButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Button_BY) && bButton.activeSelf)
        {
            bButton.SetActive(false);
        }
    }

    public override void OnActuation(HandController.Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(HandController.Actuation actuation)
    {
        base.SetActuation(actuation);

        SetOculusButtonState(actuation);
        SetAButtonState(actuation);
        SetBButtonState(actuation);
    }

    public override void OnThumbstickRaw(Vector2 value, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnThumbstickRaw:Value : {value}");
            SetThumbstickRaw(value);
        }
    }

    public override void OnState(HandStateCanvasManager.State handGesture, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.RightHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnHandGesture:HandGesture : {handGesture}");
            SetState(handGesture);
        }
    }
}