using UnityEngine;

public class HipDocksManager : BaseManager
{
    [Header("Docks")]
    [SerializeField] DockManager leftDock;
    [SerializeField] DockManager rightDock;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    public bool DockWeapon(GameObject gameObject, NavId dockID, Quaternion localRotation, bool allowNegotiation = true)
    {
        Transform parent = null;
        bool docked = false;

        if (!(docked = TryDockObject(gameObject, dockID, localRotation, out parent)) && allowNegotiation)
        {
            dockID = (dockID == NavId.Left) ? NavId.Right : NavId.Left;
            docked = TryDockObject(gameObject, dockID, localRotation, out parent);
        }

        return docked;
    }

    private bool TryDockObject(GameObject gameObject, NavId dockID, Quaternion localRotation, out Transform parent)
    {
        switch (dockID)
        {
            case NavId.Left:
                if (!leftDock.Data.occupied)
                {
                    parent = leftDock.transform;
                    OccupyDock(dockID, gameObject, parent, localRotation);
                    return true;
                }
                break;

            case NavId.Right:
                if (!rightDock.Data.occupied)
                {
                    parent = rightDock.transform;
                    OccupyDock(dockID, gameObject, parent, localRotation);
                    return true;
                }
                break;
        }

        parent = default(Transform);
        return false;
    }

    public bool TryIsDocked(GameObject gameObject, out NavId dockID)
    {
        if (leftDock.Data.occupied && Object.ReferenceEquals(gameObject, leftDock.Data.gameObject))
        {
            dockID = NavId.Left;
            return true;
        }
        else if (rightDock.Data.occupied && Object.ReferenceEquals(gameObject, rightDock.Data.gameObject))
        {
            dockID = NavId.Right;
            return true;
        }

        dockID = default(NavId);
        return false;
    }

    public void UndockWeapon(GameObject gameObject)
    {
        if (TryIsDocked(gameObject, out NavId dockID))
        {
            if (dockID == NavId.Left)
            {
                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                FreeDock(NavId.Left);
            }
            else if (dockID == NavId.Right)
            {
                AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);
                FreeDock(NavId.Right);
            }
        }
    }

    private void OccupyDock(NavId dockID, GameObject gameObject, Transform parent, Quaternion localRotation)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
        }

        gameObject.transform.parent = parent;
        gameObject.transform.localRotation = localRotation;
        gameObject.transform.localPosition = Vector3.zero;
        
        var data = new DockManager.OccupancyData
        {
            occupied = true,
            gameObject = gameObject
        };

        switch (dockID)
        {
            case NavId.Left:
                leftDock.Data = data;
                break;

            case NavId.Right:
                rightDock.Data = data;
                break;
        }

        AudioSource.PlayClipAtPoint(dockClip, transform.position, 1.0f);
    }

    private void FreeDock(NavId dockID) => ((dockID == NavId.Left) ? leftDock : rightDock).Free();
}