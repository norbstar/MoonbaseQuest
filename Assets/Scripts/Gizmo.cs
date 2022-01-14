using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [Header("Gizmo")]
    [SerializeField] Color color;
    [SerializeField] bool mapToWorldSpace = false;

    void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (mapToWorldSpace)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
        }
        
        Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
    }
}