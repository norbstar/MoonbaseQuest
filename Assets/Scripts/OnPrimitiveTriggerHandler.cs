using UnityEngine;

public class OnPrimitiveTriggerHandler : MonoBehaviour
{
    public enum EventType
    {
        OnTriggerEnter,
        OnTriggerExit
    }

    public delegate void Event(EventType type, GameObject gameObject);
    public event Event EventReceived;

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        
        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            EventReceived?.Invoke(EventType.OnTriggerEnter, rootGameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            EventReceived?.Invoke(EventType.OnTriggerExit, rootGameObject);
        }
    }
}