using UnityEngine;

public class AsteroidCatcher : MonoBehaviour
{
    [SerializeField] Vector3 size;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, size);
    }

    public void OnTriggerEnter(Collider collider)
    {
        var obj = collider.gameObject;

        if (obj.CompareTag("Asteroid"))
        {
            Destroy(obj);
        }
    }
}