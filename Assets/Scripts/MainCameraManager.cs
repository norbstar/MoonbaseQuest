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
        // Debug.Log($"{Time.time} {gameObject.name} 1");
        testCaseRunner.Post($"{className} 1");
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
                    // Debug.Log($"{Time.time} {gameObject.name} 2");
                    testCaseRunner.Post($"{className} 2");
                    if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
                    {
                        // Debug.Log($"{Time.time} {gameObject.name} 3");
                        testCaseRunner.Post($"{className} 3");
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
                    SetDockStatus(dockID, gameObject, true);

                    return true;
                }
                break;

            case DockID.Right:
                if (!rightDock.Occupied.occupied)
                {
                    // Debug.Log($"{Time.time} {gameObject.name} 4");
                    testCaseRunner.Post($"{className} 4");
                    if (gameObject.TryGetComponent<Rigidbody>(out var rigidBody))
                    {
                        // Debug.Log($"{Time.time} {gameObject.name} 5");
                        testCaseRunner.Post($"{className} 5");
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
                    SetDockStatus(dockID, gameObject, true);

                    return true;
                }
                break;
        }

        parent = default(Transform);
        return false;
    }

    // public bool IsDocked(GameObject gameObject)
    // {
    //     testCaseRunner.Post($"{className} 6");

    //     return
    //         (leftDock.Occupied.occupied && Object.ReferenceEquals(gameObject, leftDock.Occupied.gameObject)) ||
    //         (rightDock.Occupied.occupied && Object.ReferenceEquals(gameObject, rightDock.Occupied.gameObject));
    // }

    public bool TryIsDocked(GameObject gameObject, out DockID dockID)
    {
        testCaseRunner.Post($"{className} 6");

        if (leftDock.Occupied.occupied && Object.ReferenceEquals(gameObject, leftDock.Occupied.gameObject))
        {
            dockID = DockID.Left;
            return true;
        }
        else if (rightDock.Occupied.occupied && Object.ReferenceEquals(gameObject, rightDock.Occupied.gameObject))
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
                // Debug.Log($"{Time.time} {gameObject.name} 7");
                testCaseRunner.Post($"{className} 7");

                if (gameObject.TryGetComponent<Rigidbody>(out var rigidBody))
                {
                    // Debug.Log($"{Time.time} {gameObject.name} 8");
                    testCaseRunner.Post($"{className} 8");
                    rigidBody.isKinematic = false;
                    rigidBody.useGravity = true;
                }

                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                SetDockStatus(DockID.Left, gameObject, false);
            }
            else if (dockID == DockID.Right)
            {
                // Debug.Log($"{Time.time} {gameObject.name} 9");
                testCaseRunner.Post($"{className} 9");

                if (gameObject.TryGetComponent<Rigidbody>(out var rigidBody))
                {
                    // Debug.Log($"{Time.time} {gameObject.name} 10");
                    testCaseRunner.Post($"{className} 10");
                    rigidBody.isKinematic = false;
                    rigidBody.useGravity = true;
                }

                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                SetDockStatus(DockID.Right, gameObject, false);
            }
        }
    }

    private void SetDockStatus(DockID dockID, GameObject gameObject, bool occupied)
    {
        // Debug.Log($"{Time.time} {gameObject.name} 11");
        testCaseRunner.Post($"{className} 11");

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