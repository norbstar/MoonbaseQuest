using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
    public enum DockID
    {
        Left,
        Right
    }

    [Header("Docks")]
    [SerializeField] HandDockManager leftDock;
    [SerializeField] HandDockManager rightDock;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;
    
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

    public void DockObject(GameObject gameObject, DockID dockID)
    {
        switch (dockID)
        {
            // float newLocalScale = other.gameObject.transform.lossyScale.x / transform.lossyScale.x;
            // this.transform.parent=other.gameObject.transform;
            // this.transform.localScale = newLocalScale * Vector3.one;
            
            case DockID.Left:
                if (!leftDock.InUse)
                {
                    var newScale = new Vector3 {
                        x = leftDock.Container.transform.lossyScale.x / guns.transform.lossyScale.x,
                        y = leftDock.Container.transform.lossyScale.y / guns.transform.lossyScale.y,
                        z = leftDock.Container.transform.lossyScale.z / guns.transform.lossyScale.z
                    };

                    // var rotation = gameObject.transform.rotation;
                    gameObject.transform.localRotation = Quaternion.identity;

                    gameObject.transform.parent = leftDock.Container.transform;
                    gameObject.transform.localScale = new Vector3(newScale.x, newScale.y, newScale.z);
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    MarkDock(DockID.Left, true);
                }
                break;

            case DockID.Right:
                if (!rightDock.InUse)
                {
                    var newScale = new Vector3 {
                        x = rightDock.Container.transform.lossyScale.x / guns.transform.lossyScale.x,
                        y = rightDock.Container.transform.lossyScale.y / guns.transform.lossyScale.y,
                        z = rightDock.Container.transform.lossyScale.z / guns.transform.lossyScale.z
                    };

                    // var rotation = gameObject.transform.rotation;
                    gameObject.transform.localRotation = Quaternion.identity;

                    gameObject.transform.parent = rightDock.Container.transform;
                    gameObject.transform.localScale = new Vector3(newScale.x, newScale.y, newScale.z);
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    MarkDock(DockID.Right, true);
                }
                break;
        }
    }

    public void MarkDock(DockID dockID, bool inUse)
    {
        switch (dockID)
        {
            case DockID.Left:
                leftDock.InUse = inUse;
                break;

            case DockID.Right:
                rightDock.InUse = inUse;
                break;
        }
    }

    void FixedUpdate()
    {
        // Debug.DrawRay(transform.TransformPoint(Vector3.zero), transform.forward, Color.red);
        
        var ray = new Ray(transform.TransformPoint(Vector3.zero), transform.forward);
        Debug.DrawLine(ray.origin, ray.GetPoint(10f), Color.red, 0.1f);

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