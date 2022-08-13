using UnityEngine;

namespace FX
{
    public class PulseScaleFX : PulseFX
    {
        [Header("Custom Config")]
        [SerializeField] bool scaleZAxis = false;

        protected override void OnApply(object arg1, object arg2 = null)
        {
            var fraction = (float) arg1;
            var scale = minValue + ((maxValue - minValue) * fraction);
            
            transform.localScale = new Vector3(scale, scale, (scaleZAxis) ? scale : transform.localScale.z);
        }
    }
}