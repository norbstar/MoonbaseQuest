using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class CustomGrabbableCubeInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Spawning")]
    [SerializeField] GameObject spawnPrefab;

    void OnEnable()
    {
        HandController.RawDataEventReceived += OnRawData;
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
            var instance = SpawnPrefab(triggerValue);
            Destroy(instance, 5f);
        }
    }

    public void OnActuation(HandController.Actuation actuation, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");

        if (!IsHeld) return;

        if (actuation.HasFlag(HandController.Actuation.Button_AX))
        {
            SpawnPrefab();
        }
    }

    private GameObject SpawnPrefab(float scale = 1f)
    {
        var instance = GameObject.Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
        instance.gameObject.transform.SetParent(objects);
        instance.gameObject.transform.localScale *= scale;

        return instance;
    }
}