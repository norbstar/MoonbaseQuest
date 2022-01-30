using UnityEngine;

public interface IController
{
    void SetGestureState(HandController.Gesture gesture);

    void SetThumbStickCursor(Vector2 thumbStickValue);

    void SetHandGestureState(HandGestureCanvasManager.Gesture gesture);
}