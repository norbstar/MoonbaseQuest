using UnityEngine;

public class OnCollisionHandler : MonoBehaviour
{
    public enum EventType
    {
        OnCollisionEnter,
        OnCollisionExit
    }

    public delegate void Event(EventType type, GameObject gameObject);
    public event Event EventReceived;

    private int enterInstanceId, exitInstanceId;

    public void OnCollisonEnter(Collision collision)
    {
        GameObject trigger = collision.gameObject;
        int instanceId = trigger.GetInstanceID();

        if (instanceId != enterInstanceId)
        {
            enterInstanceId = instanceId;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                EventReceived?.Invoke(EventType.OnCollisionEnter, rootGameObject);
            }
        }
    }

    public void OnCollisonExit(Collision collision)
    {
        GameObject trigger = collision.gameObject;
        int instanceId = trigger.GetInstanceID();

        if (instanceId != exitInstanceId)
        {
            exitInstanceId = instanceId;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                EventReceived?.Invoke(EventType.OnCollisionExit, rootGameObject);
            }
        }
    }
}