using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class XStickInteractableManager : StickInteractableManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [SerializeField] float rotationForce = 10f;

    public delegate void MessageEvent(string message);
    public static event MessageEvent MessageEventReceived;

    void FixedUpdate()
    {
        Log($"{gameObject.name}.FixedUpdate:Rotation : {transform.localEulerAngles.z}");

        if (IsHeld)
        {
            float x = SignedEulerAngle(transform.localEulerAngles.x);
            // Log($"{gameObject.name}.FixedUpdate:X : {x}");

            float roundedX = Mathf.Round((x * 100f) / 100f);
            // Log($"{gameObject.name}.FixedUpdate:Rounded X : {roundedX}");
            
            float absX = Mathf.Abs(roundedX);
            // Log($"{gameObject.name}.FixedUpdate:ABS X : {absX}");
            
            float normalizedX = (absX - 0) / (25 - 0);
            // Log($"{gameObject.name}.FixedUpdate:Normalized X : {absX}");

            MessageEventReceived?.Invoke($"{absX} : {normalizedX}");

            var rotation = (x < 0) ? (normalizedX * -1) : normalizedX;
            FlightPlatformManager.Rotate(ControllerId, rotation * rotationForce, normalizedX);
        }
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");
    }
}