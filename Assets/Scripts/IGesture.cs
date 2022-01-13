public interface IGesture
{
    void OnGesture(HandController.Gesture gesture, object value = null);
}