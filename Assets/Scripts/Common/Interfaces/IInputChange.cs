using UnityEngine.XR;

using static Enum.ControllerEnums;

public interface IInputChange
{
    void OnInputChange(Input actuation, InputDeviceCharacteristics characteristics, object value = null);
}