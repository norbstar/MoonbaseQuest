using UnityEngine;

public class DamageAttributes : MonoBehaviour
{
    [SerializeField] float damageMetric;

    public virtual float GetDamageMetric()
    {
        return damageMetric;
    }

    public void SetDamageMetric(float damageMetric)
    {
        this.damageMetric = damageMetric;
    }
}