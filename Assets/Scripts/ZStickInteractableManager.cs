using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class ZStickInteractableManager : StickInteractableManager
{
    [SerializeField] float rotationForce = 10f;

    public delegate void MessageEvent(string message);
    public static event MessageEvent MessageEventReceived;

    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    void FixedUpdate()
    {
        Log($"{gameObject.name}.FixedUpdate:Rotation : {transform.localEulerAngles.z}");

        if (IsHeld)
        {
            
            float z = SignedEulerAngle(transform.localEulerAngles.z);
            // Log($"{gameObject.name}.FixedUpdate:Z : {z}");

            float roundedZ = Mathf.Round((z * 100f) / 100f);
            // Log($"{gameObject.name}.FixedUpdate:Rounded Z : {roundedZ}");
            
            float absZ = Mathf.Abs(roundedZ);
            // Log($"{gameObject.name}.FixedUpdate:ABS Z : {absZ}");
            
            float normalizedZ = (absZ - 0) / (25 - 0);
            // Log($"{gameObject.name}.FixedUpdate:Normalized Z : {absZ}");

            MessageEventReceived?.Invoke($"{absZ} : {normalizedZ}");

            var rotation = (z < 0) ? (normalizedZ * -1) : normalizedZ;
            FlightPlatformManager.Rotate(ControllerId, rotation * rotationForce, normalizedZ);
        }
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");
    }
}