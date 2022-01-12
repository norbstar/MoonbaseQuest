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

    [Header("Audio")]
    [SerializeField] AudioClip holsterClip, unholsterClip;
    
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
        Transform parent = null;

        switch (dockID)
        {
            case DockID.Left:
                if (!leftDock.InUse)
                {
                   parent = leftDock.transform;
                }
                break;

            case DockID.Right:
                if (!rightDock.InUse)
                {
                    parent = rightDock.transform;
                }
                break;
        }

        if (parent != null)
        {
            gameObject.transform.parent = parent;
            gameObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            gameObject.transform.localPosition = Vector3.zero;
            MarkDock(dockID, true);
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

        AudioSource.PlayClipAtPoint(inUse ? holsterClip : unholsterClip, transform.position, 1.0f);
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