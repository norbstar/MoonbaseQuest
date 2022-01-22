using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [Header("Gizmo")]
    [SerializeField] Color color;
    [SerializeField] Vector3 size = Vector3.one * 0.1f;
    [SerializeField] bool mapToWorldSpace = false;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (mapToWorldSpace)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
        }
        else
        {
            Gizmos.matrix = transform.worldToLocalMatrix;
        }
        
        Gizmos.DrawCube(transform.position, size);
    }
}