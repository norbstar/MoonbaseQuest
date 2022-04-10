using UnityEngine;

public class HybridHandController : HandController
{
    [Header("Colliders")]
    [SerializeField] BoxCollider boxCollider;

    protected override void IsGripping(bool gripping)
    {
        boxCollider.enabled = !gripping;
        sphereCollider.enabled = gripping;
    }
}