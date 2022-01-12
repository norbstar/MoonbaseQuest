using System.Collections;

using UnityEngine;

namespace FX
{
    public class RotateFX : BaseFX
    {
        [SerializeField] float speedX = 1.0f;
        [SerializeField] float speedY = 1.0f;
        [SerializeField] float speedZ = 1.0f;

        public float SpeedX { get { return speedX; } set { speedX = value; } }
        public float SpeedY { get { return speedY; } set { speedY = value; } }
        public float SpeedZ { get { return speedZ; } set { speedZ = value; } }

        public override IEnumerator Apply(object config = null)
        {
            while (true)
            {
                OnApply(null);
                yield return null;
            }
        }

        protected override void OnApply(object arg1, object arg2 = null)
        {
            transform.Rotate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime);
        }
    }
}