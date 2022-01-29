using UnityEngine;

namespace FX
{
    public abstract class PulseFX : PingPongFX
    {
        [SerializeField] protected float minValue = 1.0f;

        [SerializeField] protected float maxValue = 1.0f;
    }
}