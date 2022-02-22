using static Enum.ControllerEnums;

public interface IActuation
{
    void OnActuation(Actuation actuation, object value = null);
}