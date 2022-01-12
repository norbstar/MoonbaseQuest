using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [SerializeField] Color color;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
    }
}