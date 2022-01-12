using System.Collections;

using UnityEngine;

namespace FX
{
    public abstract class PingPongFX : BaseFX
    {
        [SerializeField] float duration = 1.0f;

        public override IEnumerator Apply(object config = null)
        {
            while (true)
            {
                var fraction = Mathf.PingPong(Time.time, duration);
                OnApply(fraction);
                yield return null;
            }
        }
    }
}