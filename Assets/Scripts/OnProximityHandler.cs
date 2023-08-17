using UnityEngine;

public class OnProximityHandler : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float radius = 1.5f;

    public delegate void Event(float distance, GameObject gameObject);
    public event Event EventReceived;

    public float Radius { get { return radius; } }

    private int defaultLayerMask;
    
    // void Awake() => defaultLayerMask = LayerMask.GetMask("Default");
    
    void FixedUpdate()
    {
        // bool hitDetected = Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hitInfo, 0f, defaultLayerMask);
        // Debug.Log($"Hit Detected: {hitDetected}");

        // if (hitDetected)
        // {
        //     var distance = Vector3.Distance(transform.position, hitInfo.point);
        //     Debug.Log($"Hit: {hitInfo.collider.gameObject.name} Distance: {distance}");
        // }

        // RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, 0f, defaultLayerMask);

        // foreach(RaycastHit hit in hits)
        // {
        //     var distance = Vector3.Distance(transform.position, hit.point);
        //     Debug.Log($"Hit: {hit.collider.gameObject.name} Distance: {distance}");
        // }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius/*, defaultLayerMask*/);

        foreach(Collider hit in hits)
        {
            var distance = Vector3.Distance(transform.position, hit.transform.position);
            // Debug.Log($"Hit: {hit.gameObject.name} Distance: {distance}");

            EventReceived?.Invoke(distance, hit.gameObject);
        }
    }
}
