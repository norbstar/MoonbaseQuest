using UnityEngine;

public class LeftControllerCanvasManager : MonoBehaviour, IController
{
    [Header("Components")]
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject grip;
    [SerializeField] GameObject thumbStick;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject x;
    [SerializeField] GameObject y;

    private HandController.Gesture lastGesture;

    public void SetState(HandController.Gesture gesture)
    {
        SetTriggerState(gesture);
        SetGripState(gesture);
        SetThumbStickState(gesture);
        SetMenuState(gesture);
        SetXState(gesture);
        SetYState(gesture);

        lastGesture = gesture;
    }

    private void SetTriggerState(HandController.Gesture gesture)
    {
        // trigger.SetActive(gesture.HasFlag(HandController.Gesture.Trigger));

        if (gesture.HasFlag(HandController.Gesture.Trigger) && !trigger.activeSelf)
        {
            trigger.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Trigger) && trigger.activeSelf)
        {
            trigger.SetActive(false);
        }
    }

    private void SetGripState(HandController.Gesture gesture)
    {
        // grip.SetActive(gesture.HasFlag(HandController.Gesture.Grip));

        if (gesture.HasFlag(HandController.Gesture.Grip) && !grip.activeSelf)
        {
            grip.SetActive(true);
        }

        if (!gesture.HasFlag(HandController.Gesture.Grip) && grip.activeSelf)
        {
            grip.SetActive(false);
        }
    }

    private void SetThumbStickState(HandController.Gesture gesture)
    {
        // thumbStick.SetActive(gesture.HasFlag(HandController.Gesture.ThumbStick_Up) || gesture.HasFlag(HandController.Gesture.ThumbStick_Left) || gesture.HasFlag(HandController.Gesture.ThumbStick_Right) || gesture.HasFlag(HandController.Gesture.ThumbStick_Down));

        if (gesture.HasFlag(HandController.Gesture.ThumbStick_Up) || gesture.HasFlag(HandController.Gesture.ThumbStick_Left) || gesture.HasFlag(HandController.Gesture.ThumbStick_Right) || gesture.HasFlag(HandController.Gesture.ThumbStick_Down) && !thumbStick.activeSelf)
        {
            thumbStick.SetActive(true);
        }

        if (!(gesture.HasFlag(HandController.Gesture.ThumbStick_Up) || gesture.HasFlag(HandController.Gesture.ThumbStick_Left) || gesture.HasFlag(HandController.Gesture.ThumbStick_Right) || gesture.HasFlag(HandController.Gesture.ThumbStick_Down)) && thumbStick.activeSelf)
        {
            thumbStick.SetActive(false);
        }
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