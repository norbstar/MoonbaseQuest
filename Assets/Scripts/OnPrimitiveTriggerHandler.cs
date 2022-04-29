using UnityEngine;

public class OnPrimitiveTriggerHandler : MonoBehaviour
{
    public delegate void Event(TriggerEventType type, GameObject gameObject);
    public event Event EventReceived;

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        
        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            EventReceived?.Invoke(TriggerEventType.OnTriggerEnter, rootGameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            EventReceived?.Invoke(TriggerEventType.OnTriggerExit, rootGameObject);
        }
    }
}