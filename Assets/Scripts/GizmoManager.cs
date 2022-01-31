using UnityEngine;

public class GizmoManager : BaseManager
{
    [Header("Gizmo")]
    [SerializeField] Color color;
    [SerializeField] Vector3 size = Vector3.one * 0.1f;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, size);
    }
}