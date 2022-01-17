using UnityEngine;

public abstract class ButtonFace : MonoBehaviour
{
    public enum EventType
    {
        OnEnter,
        OnStay,
        OnExit
    }
    
    public delegate void Event(GameObject gameObject, EventType type);
    public static event Event EventReceived;

    private int enterCount, stayCount, exitCount;

    public void OnTriggerEnter(Collider collider)
    {
        ++enterCount;
        GameObject trigger = collider.gameObject;
        // Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerEnter:[{enterCount}]");
    
        EventReceived(gameObject, EventType.OnEnter);
    }

    // public void OnTriggerStay(Collider collider)
    // {
    //     ++stayCount;
    //     GameObject trigger = collider.gameObject;
    //     Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerStay:[{stayCount}]");

    //     EventReceived(gameObject, State.OnStay);
    // }

    public void OnTriggerExit(Collider collider)
    {
        ++exitCount;
        GameObject trigger = collider.gameObject;
        // Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerExit:[{exitCount}]");

        EventReceived(gameObject, EventType.OnExit);
    }
}