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

    private void SetMenuButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Menu_Oculus) && !menuButton.activeSelf)
        {
            menuButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Menu_Oculus) && menuButton.activeSelf)
        {
            menuButton.SetActive(false);
        }
    }

    private void SetXButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Button_AX) && !xButton.activeSelf)
        {
            xButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Button_AX) && xButton.activeSelf)
        {
            xButton.SetActive(false);
        }
    }
    
    private void SetYButtonState(HandController.Actuation actuation)
    {
        if (actuation.HasFlag(HandController.Actuation.Button_BY) && !yButton.activeSelf)
        {
            yButton.SetActive(true);
        }

        if (!actuation.HasFlag(HandController.Actuation.Button_BY) && yButton.activeSelf)
        {
            yButton.SetActive(false);
        }
    }

    public override void OnActuation(HandController.Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");
            SetActuation(actuation);
        }
    }

    public override void SetActuation(HandController.Actuation actuation)
    {
        base.SetActuation(actuation);

        SetMenuButtonState(actuation);
        SetXButtonState(actuation);
        SetYButtonState(actuation);
    }

    public override void OnThumbstickRaw(Vector2 value, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnThumbstickRaw:Value : {value}");
            SetThumbstickRaw(value);
        }
    }

    public override void OnState(HandStateCanvasManager.State handGesture, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Log($"{Time.time} {gameObject.name} {className} OnHandGesture:HandGesture : {handGesture}");
            SetState(handGesture);
        }
    }
}