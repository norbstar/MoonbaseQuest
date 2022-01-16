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

    [Header("Focus")]
    [SerializeField] float focalRadius = 0.5f;
    [SerializeField] float nearDistance;
    [SerializeField] float farDistance;

    private GameObject hitPrefabInstance;
    private GameObject lastObjectHit;
    private int defaultLayerMask;
    private Transform guns;
    
    void Awake()
    {
        defaultLayerMask = LayerMask.GetMask("Default");
        guns = GameObject.Find("Objects/Guns").transform;
    }

    public bool DockWeapon(GameObject gameObject, DockID dockID, Quaternion localRotation, bool allowNegotiation = true)
    {
        Transform parent = null;
        bool docked = false;

        if (!(docked = TryDockObject(gameObject, dockID, localRotation, out parent)) && allowNegotiation)
        {
            dockID = (dockID == DockID.Left) ? DockID.Right : DockID.Left;
            docked = TryDockObject(gameObject, dockID, localRotation, out parent);
        }

        return docked;
    }

    private bool TryDockObject(GameObject gameObject, DockID dockID, Quaternion localRotation, out Transform parent)
    {
        switch (dockID)
        {
            case DockID.Left:
                if (!leftDock.Occupied.occupied)
                {
                    if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
                    {
                        rigidBody.velocity = Vector3.zero;
                        rigidBody.angularVelocity = Vector3.zero;
                        rigidBody.isKinematic = true;
                        rigidBody.useGravity = false;
                    }

                    parent = leftDock.transform;
            
                    gameObject.transform.parent = parent;
                    gameObject.transform.localRotation = localRotation;
                    gameObject.transform.localPosition = Vector3.zero;
                    MarkDock(dockID, gameObject, true);

                    return true;
                }
                break;

            case DockID.Right:
                if (!rightDock.Occupied.occupied)
                {
                    if (gameObject.TryGetComponent<Rigidbody>(out var rigidBody))
                    {
                        rigidBody.velocity = Vector3.zero;
                        rigidBody.angularVelocity = Vector3.zero;
                        rigidBody.isKinematic = true;
                        rigidBody.useGravity = false;
                    }

                    parent = rightDock.transform;

                    gameObject.transform.parent = parent;
                    gameObject.transform.localRotation = localRotation;
                    gameObject.transform.localPosition = Vector3.zero;
                    MarkDock(dockID, gameObject, true);

                    return true;
                }
                break;
        }

        parent = default(Transform);
        return false;
    }

    public bool UndockWeapon(GameObject gameObject)
    {
        if (leftDock.Occupied.occupied && Object.ReferenceEquals(gameObject, leftDock.Occupied.gameObject))
        {
            MarkDock(DockID.Left, gameObject, false);
            return true;
        }
        else if (rightDock.Occupied.occupied && Object.ReferenceEquals(gameObject, rightDock.Occupied.gameObject))
        {
            MarkDock(DockID.Right, gameObject, false);
            return true;
        }

        return false;
    }

    public bool IsDocked(GameObject gameObject)
    {
        return
            (leftDock.Occupied.occupied && Object.ReferenceEquals(gameObject, leftDock.Occupied.gameObject)) ||
            (rightDock.Occupied.occupied && Object.ReferenceEquals(gameObject, rightDock.Occupied.gameObject));
    }

     private void MarkDock(DockID dockID, GameObject gameObject, bool occupied)
    {
        switch (dockID)
        {
            case DockID.Left:
                leftDock.Occupied = new DockManager.OccupancyData
                {
                    occupied = occupied,
                    gameObject = (occupied) ? gameObject : null
                };
                break;

            case DockID.Right:
                rightDock.Occupied = new DockManager.OccupancyData
                {
                    occupied = occupied,
                    gameObject = (occupied) ? gameObject : null
                };
                break;
        }

        AudioSource.PlayClipAtPoint(occupied ? dockClip : undockClip, transform.position, 1.0f);
    }

    void FixedUpdate()
    {
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