using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

public class MainCameraManager : GizmoManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] HandController leftHandController;
    [SerializeField] HandController rightHandController;
    [SerializeField] HipDocksManager hipDocksManager;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Focus")]
    [SerializeField] float focalRadius = 0.5f;
    [SerializeField] float nearDistance;
    [SerializeField] float farDistance;

    [Header("Tracking")]
    [SerializeField] float scanRadius = 2f;
    [SerializeField] Color scanVolumeColor = new Color(0f, 0f, 0f, 0.5f);

    [Header("Movement")]
    [SerializeField] float speed = 10.0f;

    public HandController LeftHandController { get { return leftHandController; } }
    public HandController RightHandController { get { return rightHandController; } }
    public HipDocksManager HipDocksManager { get { return hipDocksManager; } }

    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;
    private int defaultLayerMask;
    private List<IInteractable> trackedInteractables;
    
    void Awake()
    {
        defaultLayerMask = LayerMask.GetMask("Default");
        trackedInteractables = new List<IInteractable>();

        // HandController.ThumbstickRawEventReceived += OnThumbstickRawEvent;
    }

    void FixedUpdate()
    {
#if false
        TrackInteractablesInRange(transform.position, scanRadius);
#endif

        bool hitDetected = Physics.SphereCast(transform.TransformPoint(Vector3.zero), focalRadius, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, defaultLayerMask);
        GameObject objectHit = null;
        Vector3 point = default(Vector3);
        bool isValid = false;

        if (hitDetected)
        {
            objectHit = hitInfo.transform.gameObject;
            point = hitInfo.point;

            var distanceToPoint = Vector3.Distance(transform.position, point);

            if ((distanceToPoint <= farDistance) && (distanceToPoint >= nearDistance))
            {
                isValid = true;
            }
        }

        if (isValid)
        {
            if (showHits)
            {
                if (hitPrefabInstance == null)
                {
                    hitPrefabInstance = Instantiate(hitPrefab, point, objectHit.transform.rotation);
                }
                else
                {
                    hitPrefabInstance.transform.position = point;
                    hitPrefabInstance.SetActive(true);
                }
            }

            if (!GameObject.ReferenceEquals(objectHit, lastObjectHit))
            {
                if ((lastObjectHit != null) && (lastObjectHit.TryGetComponent<IFocus>(out IFocus lastFocus)))
                {
                    lastFocus.LostFocus(gameObject);
                }

                if (objectHit.TryGetComponent<IFocus>(out IFocus focus))
                {
                    focus.GainedFocus(gameObject);
                }

                lastObjectHit = objectHit;
            }
        }
        else
        {
            if ((lastObjectHit != null) && (lastObjectHit.TryGetComponent<IFocus>(out IFocus lastFocus)))
            {
                lastFocus.LostFocus(gameObject);
            }

            hitPrefabInstance?.SetActive(false);
            lastObjectHit = null;
        }
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

    // public void OnThumbstickRawEvent(Vector2 thumbStickValue, InputDeviceCharacteristics characteristics)
    // {
    //     if (characteristics.HasFlag(InputDeviceCharacteristics.Left))
    //     {
    //         Log($"Left Hand Gesture:X : {thumbStickValue.x} Y : {thumbStickValue.y}");
    //     }
    //     else
    //     {
    //         Log($"Right Hand Gesture:X : {thumbStickValue.x} Y : {thumbStickValue.y}");

    //         Vector3 input = new Vector3(-thumbStickValue.y, 0f, thumbStickValue.x);
    //         transform.position += input * speed;
    //     }
    // }

    public bool TryGetObjectHit(out GameObject obj)
    {
        if (lastObjectHit != null)
        {
            obj = lastObjectHit;
            return true;
        }

        obj = default(GameObject);
        return false;
    }
}