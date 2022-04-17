using UnityEngine;

public class OnTriggerHandler : MonoBehaviour
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
            EventReceived?.Invoke(EventType.OnTriggerEnter, trigger);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        int instanceId = trigger.GetInstanceID();

        if (instanceId != exitInstanceId)
        {
            exitInstanceId = instanceId;
            EventReceived?.Invoke(EventType.OnTriggerExit, trigger);
        }
    }
}