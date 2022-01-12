using UnityEngine;

public class InConeValidator : MonoBehaviour
{
    [SerializeField] float threshold;

    public bool IsInCone(Vector3 point)
    {
        float cosAngle = Vector3.Dot((point - transform.position).normalized, transform.forward);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

        return angle < threshold;
    }
}