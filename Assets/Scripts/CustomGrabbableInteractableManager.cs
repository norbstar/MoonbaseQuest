using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class CustomGrabbableInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Spawning")]
    [SerializeField] GameObject spawnPrefab;

    private GameObject brush;
    private Vector3 originalScale, minScale, maxScale, unitScale;

    void OnEnable()
    {
        HandController.RawDataEventReceived += OnRawData;
        
        brush = SpawnBrush();
        var brushScale = brush.transform.localScale;

        var normalizedScale = new Vector3
        {
            x = (1f / transform.parent.lossyScale.x) * brushScale.x,
            y = (1f / transform.parent.lossyScale.y) * brushScale.y,
            z = (1f / transform.parent.lossyScale.z) * brushScale.z,
        };

        brush.transform.localScale = normalizedScale;
        originalScale = brush.transform.localScale;
        minScale = originalScale / 5f;
        maxScale = originalScale * 5f;
        unitScale = (maxScale - minScale) / 10f;
    }

    void OnDisable()
    {
        HandController.RawDataEventReceived -= OnRawData;
    }

    private void OnRawData(HandController.RawData rawData, InputDeviceCharacteristics characteristics)
    {
        Log($"{Time.time} {gameObject.name} {className} OnRawData");

        if (!IsHeld) return;

        var triggerValue = rawData.triggerValue;

        if (triggerValue >= 0.1f)
        {
            SpawnPrefab(triggerValue);
        }

        var velocity = rawData.thumbstickValue.x;

        if (velocity <= -0.1f || velocity >= 0.1f)
        {
            brush.transform.localScale = new Vector3
            {
                x = Mathf.Clamp(brush.transform.localScale.x + (unitScale.x * velocity), minScale.x, maxScale.x),
                y = Mathf.Clamp(brush.transform.localScale.y + (unitScale.y * velocity), minScale.y, maxScale.y),
                z = Mathf.Clamp(brush.transform.localScale.z + (unitScale.z * velocity), minScale.z, maxScale.z)
            };
        }
    }

    public void OnActuation(HandController.Actuation actuation, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");

        if (!IsHeld) return;

        if (actuation.HasFlag(HandController.Actuation.Button_AX))
        {
            var name = $"Temp_{gameObject.GetInstanceID()}";
            var instanceHierarchy = GameObject.Find(name);

            if (instanceHierarchy != null)
            {
                // for (int itr = 0; itr < instanceHierarchy.transform.childCount; itr++)
                // {
                //     Destroy(instanceHierarchy.transform.GetChild(itr).gameObject);
                // }

                Destroy(instanceHierarchy);
            }
        }
    }

    private GameObject SpawnBrush()
    {
        GameObject instance = null;

        if (spawnPrefab != null)
        {
            instance = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
            instance.transform.SetParent(transform);
        }

        return instance;
    }

    private GameObject SpawnPrefab(float scale = 1f)
    {
        GameObject instance = null;

        if (spawnPrefab != null)
        {
            var name = $"Temp_{gameObject.GetInstanceID()}";
            var instanceHierarchy = GameObject.Find(name);

            if (instanceHierarchy == null)
            {
                instanceHierarchy = new GameObject(name);
            }

            instance = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
            instance.transform.SetParent(instanceHierarchy.transform);
            instance.transform.localScale = brush.transform.localScale * scale;
        }

        return instance;
    }
}