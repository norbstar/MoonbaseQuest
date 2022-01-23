using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ProximityBaseController))]
public class ObservableUI : MonoBehaviour
{
    [SerializeField] Image image;

    [SerializeField] float nearDistance = 0.5f;
    [SerializeField] float farDistance = 2.5f;

    public float NearDistance { get { return nearDistance; } }
    public float FarDistance { get { return farDistance; } }

    protected ProximityBaseController baseController;
    protected float distance;
    protected bool inRange;
    
    private Vector3 originalScale, baseScale;
    
    public virtual void Awake()
    {
        ResolveDependencies();

        originalScale = transform.localScale;
        var referenceScale = baseController.Origin.transform.localScale;

        baseScale = new Vector3
        {
            x = 1f / (referenceScale.x / 0.1f),
            y = 1f / (referenceScale.y / 0.1f),
            z = 1f / (referenceScale.z / 0.1f)
        };

        transform.localScale = baseScale;
    }

    private void ResolveDependencies()
    {
        baseController = GetComponent<ProximityBaseController>() as ProximityBaseController;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        distance = Vector3.Distance(baseController.Origin.transform.position, baseController.Camera.transform.position);
        inRange = (distance >= nearDistance && distance <= farDistance);
        baseController.InRange = inRange;
    }
}