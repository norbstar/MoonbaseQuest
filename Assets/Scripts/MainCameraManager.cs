using System.Reflection;

using UnityEngine;

public class MainCameraManager : Gizmo
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

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
                if (!leftDock.Data.occupied)
                {
                    OccupyDock(dockID, gameObject);

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

                    AudioSource.PlayClipAtPoint(dockClip, transform.position, 1.0f);

                    return true;
                }
                break;

            case DockID.Right:
                if (!rightDock.Data.occupied)
                {
                    OccupyDock(dockID, gameObject);

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

                    AudioSource.PlayClipAtPoint(dockClip, transform.position, 1.0f);

                    return true;
                }
                break;
        }

        parent = default(Transform);
        return false;
    }

    public bool TryIsDocked(GameObject gameObject, out DockID dockID)
    {
        if (leftDock.Data.occupied && Object.ReferenceEquals(gameObject, leftDock.Data.gameObject))
        {
            dockID = DockID.Left;
            return true;
        }
        else if (rightDock.Data.occupied && Object.ReferenceEquals(gameObject, rightDock.Data.gameObject))
        {
            dockID = DockID.Right;
            return true;
        }

        dockID = default(DockID);
        return false;
    }

    public void UndockWeapon(GameObject gameObject)
    {
        if (TryIsDocked(gameObject, out DockID dockID))
        {
            if (dockID == DockID.Left)
            {
                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                FreeDock(DockID.Left);
            }
            else if (dockID == DockID.Right)
            {
                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                FreeDock(DockID.Right);
            }
        }
    }

    private void OccupyDock(DockID dockID, GameObject gameObject)
    {
        var data = new DockManager.OccupancyData
        {
            occupied = true,
            gameObject = gameObject
        };

        switch (dockID)
        {
            case DockID.Left:
                leftDock.Data = data;
                break;

            case DockID.Right:
                rightDock.Data = data;
                break;
        }
    }

    private void FreeDock(DockID dockID)
    {
        var dock = (dockID == DockID.Left) ? leftDock : rightDock; 
        dock.Free();
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