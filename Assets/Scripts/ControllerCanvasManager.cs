using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class ControllerCanvasManager : MonoBehaviour, IController
{
    [Header("Manager")]
    [SerializeField] HandGestureCanvasManager handGestureCanvasManager;

    [Header("Components")]
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject grip;
    [SerializeField] GameObject thumbStick;
    [SerializeField] GameObject thumbStickClick;
    [SerializeField] GameObject thumbStickCursor;

    private HandController.Gesture lastGesture;

    public virtual void SetGestureState(HandController.Gesture gesture)
    {
        lastGesture = gesture;

        SetTriggerState(gesture);
        SetGripState(gesture);
        SetThumbStickState(gesture);
        SetThumbStickClickState(gesture);
    }

    [Header("Optional Settings")]
    [SerializeField] bool enableThumbStickCursor;

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

    public void SetThumbStickCursor(Vector2 thumbStickValue)
    {
        if (!enableThumbStickCursor) return;

        float x = thumbStickValue.x * 4f;
        float y = thumbStickValue.y * 4f;

        if (!thumbStickCursor.activeSelf)
        {
            thumbStickCursor.SetActive(true);
        }

        thumbStickCursor.transform.GetChild(0).localPosition = new Vector3(x, y, 0f);
    }

    private void SetThumbStickClickState(HandController.Gesture gesture)
    {
        // thumbStickClick.SetActive(gesture.HasFlag(HandController.Gesture.ThumbStick_Click));

        if (gesture.HasFlag(HandController.Gesture.ThumbStick_Click) && !thumbStickClick.activeSelf)
        {
            thumbStickClick.SetActive(true);
        }

        if (!(gesture.HasFlag(HandController.Gesture.ThumbStick_Click) && thumbStickClick.activeSelf))
        {
            thumbStickClick.SetActive(false);
        }
    }

    public void SetHandGestureState(HandGestureCanvasManager.Gesture gesture)
    {
        handGestureCanvasManager.SetGestureState(gesture);
    }
}