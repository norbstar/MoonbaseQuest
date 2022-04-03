using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class ZStickInteractableManager : StickInteractableManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    void FixedUpdate()
    {
        Log($"{gameObject.name}.FixedUpdate:Rotation : {transform.rotation.eulerAngles}");

        if (IsHeld)
        {
            // TODO
        }
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");
    }
}