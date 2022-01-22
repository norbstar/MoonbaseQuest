using System.Reflection;

using UnityEngine;

public class MainCameraManager : Gizmo
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] HipDocksManager hipDocksManager;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Focus")]
    [SerializeField] float focalRadius = 0.5f;
    [SerializeField] float nearDistance;
    [SerializeField] float farDistance;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public HipDocksManager HipDocksManager { get { return hipDocksManager; } }

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

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}