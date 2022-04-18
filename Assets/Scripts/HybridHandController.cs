using UnityEngine;

public class HybridHandController : HandController
{
    [Header("Components")]
    [SerializeField] GameObject body;
    [SerializeField] SpawnPointManager spawnPoint;

    [Header("Hits")]
    [SerializeField] bool showHits;
    protected bool ShowHits { get { return showHits; } }
    
    [SerializeField] GameObject hitPrefab;
    protected GameObject HitPrefab { get { return hitPrefab; } }

    [Header("Focus")]
    [SerializeField] float nearDistance;
    protected float NearDistance { get { return nearDistance; } }

    [SerializeField] float farDistance;
    protected float FarDistance { get { return farDistance; } }

    [Header("Config")]
    [SerializeField] private bool enableTracking = true;

    private MeshCollider meshCollider;
    private OnCollisionHandler collisionHandler;
    private int interactableLayerMask;
    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;

    public override void Awake()
    {
        base.Awake();
        ResolveLocalDependencies();

        interactableLayerMask = LayerMask.GetMask("Interactable Layer");
    }

    private void ResolveLocalDependencies()
    {
        collisionHandler = body.GetComponent<OnCollisionHandler>() as OnCollisionHandler;
        meshCollider = body.GetComponent<MeshCollider>() as MeshCollider;
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


    void OnEnable()
    {
        collisionHandler.EventReceived += OnCollisionEvent;
    }

    void OnDisable()
    {
        collisionHandler.EventReceived -= OnCollisionEvent;
    }

    public void OnCollisionEvent(OnCollisionHandler.EventType type, GameObject gameObject)
    {
        switch (type)
        {
            case OnCollisionHandler.EventType.OnCollisionEnter:
                break;

            case OnCollisionHandler.EventType.OnCollisionExit:
                break;
        }
    }

    void FixedUpdate()
    {
        if (!enableTracking) return;

        var ray = new Ray(spawnPoint.transform.position, transform.forward);
        bool hitDetected = Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, farDistance, interactableLayerMask);
        GameObject objectHit = null;
        Vector3 point = default(Vector3);
        Vector3 vector = Vector3.zero;

        bool isValid = false;

        if (hitDetected)
        {
            objectHit = hit.transform.gameObject;
            point = hit.point;
            vector = (point - spawnPoint.transform.position).normalized;

            float distanceToPoint = Vector3.Distance(transform.position, point);
            isValid = (distanceToPoint >= nearDistance);
        }

        if (!isValid)
        {
            if ((lastObjectHit != null) && (lastObjectHit.TryGetComponent<IFocus>(out IFocus lastFocus)))
            {
                lastFocus.LostFocus(gameObject);
            }

            hitPrefabInstance?.SetActive(false);
            spawnPoint.EnableLine = false;
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
            }

            hitPrefabInstance.SetActive(true);                
            spawnPoint.ConfigLine(point, vector);
            spawnPoint.EnableLine = true;
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
    
    protected override void IsGripping(bool gripping)
    {
        meshCollider.enabled = !gripping;
        sphereCollider.enabled = gripping;
    }
}