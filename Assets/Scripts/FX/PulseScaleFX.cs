using UnityEngine;

namespace FX
{
    public class PulseScaleFX : PulseFX
    {
        protected override void OnApply(object arg1, object arg2 = null)
        {
            var fraction = (float) arg1;
            var scale = minValue + ((maxValue - minValue) * fraction);
            transform.localScale = new Vector3(scale, scale, transform.localScale.z);
        }
    }
}