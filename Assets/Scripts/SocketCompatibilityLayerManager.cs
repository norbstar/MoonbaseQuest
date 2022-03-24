using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SocketCompatibilityLayerManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public enum EventType
    {
        OnTriggerEnter,
        OnTriggerExit
    }

    [Header("Config")]
    [SerializeField] string tagName;

    public delegate void Event(SocketCompatibilityLayerManager manager, EventType type, GameObject gameObject);
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

            if (!rootGameObject.tag.Equals(tagName)) return;
        
            if (EventReceived != null)
            {
                EventReceived(this, EventType.OnTriggerEnter, rootGameObject);
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            if (!rootGameObject.tag.Equals(tagName)) return;

            Log($"{Time.time} {gameObject.name} {className} OnTriggerExit:GameObject : {rootGameObject.name}");

            if (EventReceived != null)
            {
                EventReceived(this, EventType.OnTriggerExit, rootGameObject);
            }
        }
    }
}