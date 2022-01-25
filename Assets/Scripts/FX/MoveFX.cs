using UnityEngine;

namespace FX
{
    public class MoveFX : PingPongFX
    {
        [SerializeField] protected Vector3 start;

        [SerializeField] protected Vector3 end;

        protected override void OnApply(object arg1, object arg2 = null)
        {
            float fraction = (float) arg1;
            transform.transform.position = Vector3.Lerp(start, end, fraction);
        }
    }
}