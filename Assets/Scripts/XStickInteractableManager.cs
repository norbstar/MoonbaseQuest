using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

public class XStickInteractableManager : StickInteractableManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [SerializeField] float rotationForce = 10f;

    [SerializeField] TextMeshProCanvasManager statsCanvas;

    // void Start()
    // {
    //     float value = -25;
    //     var normalized = NormalizeAxis(value);
    //     Log($"Value : {value} Normalized Value : {normalized}");
    //     value = 0;
    //     normalized = NormalizeAxis(value);
    //     Log($"Value : {value} Normalized Value : {normalized}");
    //     value = 25;
    //     normalized = NormalizeAxis(value);
    //     Log($"Value : {value} Normalized Value : {normalized}");
    //     value = transform.rotation.z;
    //     normalized = NormalizeAxis(value);
    //     Log($"Value : {value} Normalized Value : {normalized}");
    // }

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

            statsCanvas.Text = $"{absZ} : {normalizedZ}";

            var rotation = (z < 0) ? (normalizedZ * -1) : normalizedZ;
            FlightPlatformManager.Rotate(rotation * rotationForce, normalizedZ);
        }
    }

    public float SignedEulerAngle(float eulerAngle)
    {
        if (eulerAngle >= 180)
        {
            eulerAngle = (360f - eulerAngle) * -1;
        }

        return eulerAngle;
    }

    public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");
    }
}