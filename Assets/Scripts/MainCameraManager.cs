using System.Reflection;

using UnityEngine;

public class MainCameraManager : Gizmo
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] HipDocksManager hipDocksManager;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Focus")]
    [SerializeField] float focalRadius = 0.5f;
    [SerializeField] float nearDistance;
    [SerializeField] float farDistance;

    [Header("Scanning")]
    [SerializeField] float scanRadius = 2f;
    [SerializeField] Color scanVolumeColor = new Color(0f, 0f, 0f, 0.5f);

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public HipDocksManager HipDocksManager { get { return hipDocksManager; } }

    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;
    private int defaultLayerMask;
    private TestCaseRunner testCaseRunner;
    
    void Awake()
    {
        ResolveDependencies();
        defaultLayerMask = LayerMask.GetMask("Default");
    }

    private void ResolveDependencies()
    {
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    void FixedUpdate()
    {
        IdentifyInteractablesInRange(transform.position, scanRadius);

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

    private void IdentifyInteractablesInRange(Vector3 center, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);
       
        foreach (var hit in hits)
        {
            GameObject trigger = hit.gameObject;

            if (TryGetInteractable<InteractableManager>(trigger, out InteractableManager interactable))
            {
                Debug.Log($"Detected {trigger.name}");
            }
        }
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Gizmos.color = scanVolumeColor;
        // Gizmos.matrix = transform.localToWorldMatrix;
        
        // Gizmos.DrawSphere(transform.position, scanRadius);
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}