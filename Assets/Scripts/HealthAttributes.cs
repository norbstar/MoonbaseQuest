using UnityEngine;

public class HealthAttributes : MonoBehaviour
{
    [SerializeField] float healthMetric;
    [SerializeField] bool invulnerable;

    public float GetHealthMetric()
    {
        return healthMetric;
    }

    public void SubstractHealth(float healthMetric)
    {
        if (!invulnerable)
        {
            this.healthMetric -= healthMetric;
        }
    }
}