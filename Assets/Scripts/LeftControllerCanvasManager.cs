using UnityEngine;

public class LeftControllerCanvasManager : ControllerCanvasManager
{
    [Header("Other Components")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject x;
    [SerializeField] GameObject y;

    public override void SetGestureState(HandController.Gesture gesture)
    {
        base.SetGestureState(gesture);

        SetMenuState(gesture);
        SetXState(gesture);
        SetYState(gesture);
    }

    private void SetMenuState(HandController.Gesture gesture)
    {
        // menu.SetActive(gesture.HasFlag(HandController.Gesture.Menu_Oculus));

        if (gesture.HasFlag(HandController.Gesture.Menu_Oculus) && !menu.activeSelf)
        {
            menu.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Menu_Oculus) && menu.activeSelf)
        {
            menu.SetActive(false);
        }
    }

    private void SetXState(HandController.Gesture gesture)
    {
        // x.SetActive(gesture.HasFlag(HandController.Gesture.Button_AX));

        if (gesture.HasFlag(HandController.Gesture.Button_AX) && !x.activeSelf)
        {
            x.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Button_AX) && x.activeSelf)
        {
            x.SetActive(false);
        }
    }
    
    private void SetYState(HandController.Gesture gesture)
    {
        // y.SetActive(gesture.HasFlag(HandController.Gesture.Button_BY));

        if (gesture.HasFlag(HandController.Gesture.Button_BY) && !y.activeSelf)
        {
            y.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Button_BY) && y.activeSelf)
        {
            y.SetActive(false);
        }
    }
}