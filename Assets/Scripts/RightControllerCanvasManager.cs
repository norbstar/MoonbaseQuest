using UnityEngine;

public class RightControllerCanvasManager : ControllerCanvasManager
{
    [Header("Other Components")]
    [SerializeField] GameObject oculus;
    [SerializeField] GameObject a;
    [SerializeField] GameObject b;

    public override void SetGestureState(HandController.Gesture gesture)
    {
        base.SetGestureState(gesture);

        SetOculusState(gesture);
        SetAState(gesture);
        SetBState(gesture);
    }

    private void SetOculusState(HandController.Gesture gesture)
    {
        // menu.SetActive(gesture.HasFlag(HandController.Gesture.Menu_Oculus));

        if (gesture.HasFlag(HandController.Gesture.Menu_Oculus) && !oculus.activeSelf)
        {
            oculus.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Menu_Oculus) && oculus.activeSelf)
        {
            oculus.SetActive(false);
        }
    }

    private void SetAState(HandController.Gesture gesture)
    {
        // x.SetActive(gesture.HasFlag(HandController.Gesture.Button_AX));

        if (gesture.HasFlag(HandController.Gesture.Button_AX) && !a.activeSelf)
        {
            a.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Button_AX) && a.activeSelf)
        {
            a.SetActive(false);
        }
    }
    
    private void SetBState(HandController.Gesture gesture)
    {
        // y.SetActive(gesture.HasFlag(HandController.Gesture.Button_BY));

        if (gesture.HasFlag(HandController.Gesture.Button_BY) && !b.activeSelf)
        {
            b.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Button_BY) && b.activeSelf)
        {
            b.SetActive(false);
        }
    }
}