using UnityEngine;

public class MainCameraManager : Gizmo
{
    public enum DockID
    {
        Left,
        Right
    }

    [Header("Docks")]
    [SerializeField] DockManager leftDock;
    [SerializeField] DockManager rightDock;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    [Header("Identity")]
    [SerializeField] float nearClippingDistance;
    [SerializeField] float farClippingDistance;

    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;
    private IFocus lastFocus;
    private int defaultLayerMask;
    private Transform guns;
    
    void Awake()
    {
        defaultLayerMask = LayerMask.GetMask("Default");
        guns = GameObject.Find("Objects/Guns").transform;
    }

    public void DockObject(GameObject gameObject, DockID dockID, bool allowNegotiation = true)
    {
        Transform parent = null;

        if (TryDockObject(gameObject, dockID, out parent))
        {
            gameObject.transform.parent = parent;
            gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            gameObject.transform.localPosition = Vector3.zero;
            MarkDock(dockID, true);
        }
        else if (allowNegotiation)
        {
            dockID = (dockID == DockID.Left) ? DockID.Right : DockID.Left;

            if (TryDockObject(gameObject, dockID, out parent))
            {
                gameObject.transform.parent = parent;
                gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                gameObject.transform.localPosition = Vector3.zero;
                MarkDock(dockID, true);
            }
        }
    }

    private bool TryDockObject(GameObject gameObject, DockID dockID, out Transform parent)
    {
        switch (dockID)
        {
            case DockID.Left:
                if (!leftDock.Occupied)
                {
                    if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
                    {
                        rigidBody.velocity = Vector3.zero;
                        rigidBody.angularVelocity = Vector3.zero;
                        rigidBody.isKinematic = true;
                        rigidBody.useGravity = false;
                    }

                    parent = leftDock.transform;
                    return true;
                }
                break;

            case DockID.Right:
                if (!rightDock.Occupied)
                {
                    if (gameObject.TryGetComponent<Rigidbody>(out var rigidBody))
                    {
                        rigidBody.velocity = Vector3.zero;
                        rigidBody.angularVelocity = Vector3.zero;
                        rigidBody.isKinematic = true;
                        rigidBody.useGravity = false;
                    }

                    parent = rightDock.transform;
                    return true;
                }
                break;
        }

        parent = default(Transform);
        return false;
    }

    public void MarkDock(DockID dockID, bool occupied)
    {
        switch (dockID)
        {
            case DockID.Left:
                leftDock.Occupied = occupied;
                break;

            case DockID.Right:
                rightDock.Occupied = occupied;
                break;
        }

        Debug.Log($"Left Dock:Occupied : {leftDock.Occupied}");
        Debug.Log($"Right Dock:Occupied : {rightDock.Occupied}");

        AudioSource.PlayClipAtPoint(occupied ? dockClip : undockClip, transform.position, 1.0f);
    }

    void FixedUpdate()
    {
        // Debug.DrawRay(transform.TransformPoint(Vector3.zero), transform.forward, Color.red);
        
        var ray = new Ray(transform.TransformPoint(Vector3.zero), transform.forward);
        // Debug.DrawLine(ray.origin, ray.GetPoint(10f), Color.red, 0.1f);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, defaultLayerMask))
        {
            var objectHit = hit.transform.gameObject;
            // Debug.Log($"{gameObject.name}.Hit:{objectHit.name}");

            var point = hit.point;
            // Debug.Log($"{gameObject.name}.Point:{point}");

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
                if (lastFocus != null)
                {
                    lastFocus.LostFocus(gameObject);
                    lastFocus = null;
                }

                if (objectHit.TryGetComponent<IFocus>(out IFocus focus))
                {
                    focus.GainedFocus(gameObject);
                    lastFocus = focus;

                    // var distanceToPoint = Vector3.Distance(transform.position, point);

                    // if ((distanceToPoint <= farClippingDistance) && (distanceToPoint >= nearClippingDistance))
                    // {
                    //     focus.GainedFocus(gameObject);
                    //     lastFocus = focus;
                    // }
                }

                lastObjectHit = objectHit;
            }
        }
        else
        {
            if (lastFocus != null)
            {
                lastFocus.LostFocus(gameObject);
                lastFocus = null;
            }

            hitPrefabInstance?.SetActive(false);
            lastObjectHit = null;
        }
    }
}