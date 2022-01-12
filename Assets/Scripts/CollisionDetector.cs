using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private int enterCount, stayCount, exitCount;

    public void OnCollisonEnter(Collision collision)
    {
        ++enterCount;
        GameObject trigger = collision.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnCollisionEnter:[{enterCount}]");
    }

    public void OnTriggerEnter(Collider collider)
    {
        ++enterCount;
        GameObject trigger = collider.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerEnter:[{enterCount}]");
    }

    public void OnCollisonStay(Collision collision)
    {
        ++stayCount;
        GameObject trigger = collision.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnCollisionStay:[{stayCount}]");
    }

    public void OnTriggerStay(Collider collider)
    {
        ++stayCount;
        GameObject trigger = collider.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerStay:[{stayCount}]");
    }

    public void OnCollisonExit(Collision collision)
    {
        ++exitCount;
        GameObject trigger = collision.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnCollisionExit:[{exitCount}]");
    }

    public void OnTriggerExit(Collider collider)
    {
        ++exitCount;
        GameObject trigger = collider.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerExit:[{exitCount}]");
    }
}
