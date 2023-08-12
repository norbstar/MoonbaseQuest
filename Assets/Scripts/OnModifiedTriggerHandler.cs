using UnityEngine;

public class OnModifiedTriggerHandler : MonoBehaviour
{
    public enum EventType
    {
        OnTriggerEnter,
        OnTriggerExit
    }

    public delegate void Event(EventType type, GameObject gameObject);
    public event Event EventReceived;

    private int enterInstanceId, exitInstanceId;

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        int instanceId = trigger.GetInstanceID();
        
        if (instanceId != enterInstanceId)
        {
            enterInstanceId = instanceId;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                EventReceived?.Invoke(EventType.OnTriggerEnter, rootGameObject);
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        int instanceId = trigger.GetInstanceID();

        if (instanceId != exitInstanceId)
        {
            exitInstanceId = instanceId;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                EventReceived?.Invoke(EventType.OnTriggerExit, rootGameObject);
            }
        }
    }
}