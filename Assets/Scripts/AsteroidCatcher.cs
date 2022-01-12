using UnityEngine;

public class AsteroidCatcher : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        var obj = collider.gameObject;

        if (obj.CompareTag("Asteroid"))
        {
            Destroy(obj);
        }
    }
}