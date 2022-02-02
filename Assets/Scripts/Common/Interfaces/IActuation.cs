public interface IActuation
{
    void OnActuation(HandController.Actuation actuation, object value = null);
}