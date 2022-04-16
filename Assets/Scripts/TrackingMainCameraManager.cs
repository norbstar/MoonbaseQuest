using System.Reflection;

using UnityEngine;

public class TrackingMainCameraManager : GizmoManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Hits")]
    [SerializeField] bool showHits;
    protected bool ShowHits { get { return showHits; } } 

    [SerializeField] GameObject hitPrefab;
    protected GameObject HitPrefab { get { return hitPrefab; } } 

    [Header("Focus")]
    [SerializeField] float focalRadius = 0.5f;
    protected float FocalRadius { get { return focalRadius; } }

    [SerializeField] float nearDistance;
    protected float NearDistance { get { return nearDistance; } }

    [SerializeField] float farDistance;
    protected float FarDistance { get { return farDistance; } }
    
    [Header("Config")]
    [SerializeField] private bool enableTracking = true;

    private int interactableLayerMask;
    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;

    public virtual void Awake()
    {
        interactableLayerMask = LayerMask.GetMask("Interactable Layer");
    }

    public bool EnableTracking
    {
        get
        {
            return enableTracking;
        }

        set
        {
            enableTracking = value;
        }
    }

    public void IncludeInteractableLayer(string interactableLayer) => interactableLayerMask |= LayerMask.GetMask(interactableLayer);

    public void ExcludeInteractableLayer(string interactableLayer) => interactableLayerMask &= ~LayerMask.GetMask(interactableLayer);

    void FixedUpdate()
    {
        if (!enableTracking) return;

#if true
        var ray = new Ray(transform.position, transform.forward);
        bool hitDetected = Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, farDistance, interactableLayerMask);
        GameObject objectHit = null;
        Vector3 point = default(Vector3);

        bool isValid = false;

        if (hitDetected)
        {
            objectHit = hit.transform.gameObject;
            point = hit.point;
            var distanceToPoint = Vector3.Distance(transform.position, point);
            isValid = (distanceToPoint >= nearDistance);
        }
        
#endif

#if false
        bool hitDetected = Physics.SphereCast(transform.position/*transform.TransformPoint(Vector3.zero)*/, focalRadius, transform.forward, out RaycastHit hitInfo, farDistance, interactableLayerMask);
        GameObject objectHit = null;
        Vector3 point = default(Vector3);
        
        bool isValid = false;

        if (hitDetected)
        {
            objectHit = hitInfo.transform.gameObject;
            point = hitInfo.point;
            var distanceToPoint = Vector3.Distance(transform.position, point);
            isValid = (distanceToPoint >= nearDistance);
        }
#endif

        if (!isValid)
        {
            if ((lastObjectHit != null) && (lastObjectHit.TryGetComponent<IFocus>(out IFocus lastFocus)))
            {
                lastFocus.LostFocus(gameObject);
            }

            hitPrefabInstance?.SetActive(false);
            lastObjectHit = null;
            return;
        }

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