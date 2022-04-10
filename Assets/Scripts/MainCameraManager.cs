using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
// using UnityEngine.XR;

public class MainCameraManager : TrackingMainCameraManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] HandController leftHandController;
    [SerializeField] HandController rightHandController;
    [SerializeField] HipDocksManager hipDocksManager;

    [Header("Tracking")]
    [SerializeField] float scanRadius = 2f;
    [SerializeField] Color scanVolumeColor = new Color(0f, 0f, 0f, 0.5f);

    [Header("Movement")]
    [SerializeField] float speed = 10.0f;

    public HandController LeftHandController { get { return leftHandController; } }
    public HandController RightHandController { get { return rightHandController; } }
    public HipDocksManager HipDocksManager { get { return hipDocksManager; } }

    private List<IInteractable> trackedInteractables;
    
    public override void Awake()
    {
        base.Awake();

        trackedInteractables = new List<IInteractable>();
        // HandController.ThumbstickRawEventReceived += OnThumbstickRawEvent;
    }

#if false
    private void TrackInteractablesInRange(Vector3 center, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);
        List<IInteractable> verifiedHits = new List<IInteractable>();

        foreach (var hit in hits)
        {
            GameObject trigger = hit.gameObject;

            if (TryGet.TryGetInteractable<IInteractable>(trigger, out IInteractable interactable))
            {
                verifiedHits.Add(interactable);
            }
        }

        IEnumerable<IInteractable> obsoleteHits = trackedInteractables.ToArray<IInteractable>().Except(verifiedHits);
        obsoleteHits.ToList().ForEach(h => h.EnableTracking(false));
        
        IEnumerable<IInteractable> newHits = verifiedHits.ToArray<IInteractable>().Except(trackedInteractables);
        newHits.ToList().ForEach(h => h.EnableTracking(true));

        trackedInteractables = verifiedHits;
    }
#endif

    // public void OnThumbstickRawEvent(Vector2 value, InputDeviceCharacteristics characteristics)
    // {
    //     if (characteristics.HasFlag(InputDeviceCharacteristics.Left))
    //     {
    //         Log($"Left Hand Gesture:X : {value.x} Y : {value.y}");
    //     }
    //     else
    //     {
    //         Log($"Right Hand Gesture:X : {value.x} Y : {value.y}");

    //         Vector3 input = new Vector3(-value.y, 0f, value.x);
    //         transform.position += input * speed;
    //     }
    // }
}