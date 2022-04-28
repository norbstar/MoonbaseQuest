using UnityEngine;

public class TestCubeManager : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        var trigger = collider.gameObject;
        
        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            Debug.Log($"OnTriggerEnter : {rootGameObject.name}");
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            Debug.Log($"OnTriggerExit : {rootGameObject.name}");
        }
    }
}