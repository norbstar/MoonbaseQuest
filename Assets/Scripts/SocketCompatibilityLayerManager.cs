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
        Log($"{Time.time} {gameObject.name} {className} OnTriggerEnter:GameObject : {trigger.name}");

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnTriggerEnter, trigger);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var trigger = collider.gameObject;
        Log($"{Time.time} {gameObject.name} {className} OnTriggerExit:GameObject : {trigger.name}");

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnTriggerExit, trigger);
        }
    }
}