using UnityEngine.XR;

using static Enum.ControllerEnums;

public interface IActuation
{
    void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null);
}