using UnityEngine;

public class HipDocksManager : BaseManager
{
        public enum DockID
    {
        Left,
        Right
    }
    
    [Header("Docks")]
    [SerializeField] DockManager leftDock;
    [SerializeField] DockManager rightDock;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

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
                    parent = leftDock.transform;
                    OccupyDock(dockID, gameObject, parent, localRotation);
                    return true;
                }
                break;

            case DockID.Right:
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

    private void OccupyDock(DockID dockID, GameObject gameObject, Transform parent, Quaternion localRotation)
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
            case DockID.Left:
                leftDock.Data = data;
                break;

            case DockID.Right:
                rightDock.Data = data;
                break;
        }

        AudioSource.PlayClipAtPoint(dockClip, transform.position, 1.0f);
    }

    private void FreeDock(DockID dockID) => ((dockID == DockID.Left) ? leftDock : rightDock).Free();
}