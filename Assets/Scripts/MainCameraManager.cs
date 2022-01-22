using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

public class MainCameraManager : Gizmo
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] HandController leftHandControllerManager;
    [SerializeField] HandController rightHandControllerManager;
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

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public HandController LeftHandControllerManager { get { return leftHandControllerManager; } }
    public HandController RightHandControllerManager { get { return rightHandControllerManager; } }
    public HipDocksManager HipDocksManager { get { return hipDocksManager; } }

    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;
    private int defaultLayerMask;
    private List<InteractableManager> trackedInteractables;
    private TestCaseRunner testCaseRunner;
    
    void Awake()
    {
        ResolveDependencies();
        defaultLayerMask = LayerMask.GetMask("Default");
        trackedInteractables = new List<InteractableManager>();
    }

    private void ResolveDependencies()
    {
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    void FixedUpdate()
    {
        TrackInteractablesInRange(transform.position, scanRadius);

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

    private void TrackInteractablesInRange(Vector3 center, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);
        List<InteractableManager> verifiedHits = new List<InteractableManager>();

        foreach (var hit in hits)
        {
            GameObject trigger = hit.gameObject;

            if (TryGetInteractable<InteractableManager>(trigger, out InteractableManager interactable))
            {
                verifiedHits.Add(interactable);
            }
        }

        IEnumerable<InteractableManager> obsoleteHits = trackedInteractables.ToArray<InteractableManager>().Except(verifiedHits);
        obsoleteHits.ToList().ForEach(h => h.EnableTracking(false));
        
        IEnumerable<InteractableManager> newHits = verifiedHits.ToArray<InteractableManager>().Except(trackedInteractables);
        newHits.ToList().ForEach(h => h.EnableTracking(true));

        trackedInteractables = verifiedHits;
    }

    private bool TryGetInteractable<InteractableManager>(GameObject trigger, out InteractableManager interactable)
    {
        if (trigger.TryGetComponent<InteractableManager>(out InteractableManager interactableManager))
        {
            interactable = interactableManager;
            return true;
        }

        var component = trigger.GetComponentInParent<InteractableManager>();

        if (component != null)
        {
            interactable = component;
            return true;
        }

        interactable = default(InteractableManager);
        return false;
    }

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

    // protected override void OnDrawGizmos()
    // {
    //     base.OnDrawGizmos();

        // Gizmos.color = scanVolumeColor;
        // Gizmos.matrix = transform.localToWorldMatrix;
        
        // Gizmos.DrawSphere(transform.position, scanRadius);
    // }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}