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
            EventReceived?.Invoke(EventType.OnCollisionEnter, trigger);
        }
    }

    public void OnCollisonExit(Collision collision)
    {
        GameObject trigger = collision.gameObject;
        int instanceId = trigger.GetInstanceID();

        if (instanceId != exitInstanceId)
        {
            exitInstanceId = instanceId;
            EventReceived?.Invoke(EventType.OnCollisionExit, trigger);
        }
    }
}