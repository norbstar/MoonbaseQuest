using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SocketCompatibilityLayerManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Config")]
    [SerializeField] string tagName;

    public delegate void Event(SocketCompatibilityLayerManager manager, TriggerEventType type, GameObject gameObject);
    public event Event EventReceived;

    private new SphereCollider collider;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<SphereCollider>() as SphereCollider;
    }

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider collider)
    {
        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            Log($"{Time.time} {gameObject.name} {className} OnTriggerEnter:Root GameObject : {rootGameObject.name} Tag : {rootGameObject.tag}");

            if (!rootGameObject.CompareTag(tagName)) return;
        
            if (EventReceived != null)
            {
                EventReceived(this, TriggerEventType.OnTriggerEnter, rootGameObject);
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            if (!rootGameObject.CompareTag(tagName)) return;

            Log($"{Time.time} {gameObject.name} {className} OnTriggerExit:GameObject : {rootGameObject.name}");

            if (EventReceived != null)
            {
                EventReceived(this, TriggerEventType.OnTriggerExit, rootGameObject);
            }
        }
    }
}