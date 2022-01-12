using System;
using System.Collections;

using UnityEngine;

namespace FX
{
    public class ScaleFX : BaseFX
    {
        [SerializeField] float duration = 1.0f;

        [Serializable]
        public class Range
        {
            public float fromValue = 1.0f;
            public float toValue = 1.0f;
        }

        protected float startTransformTime;
        private bool complete;

        public override IEnumerator Apply(object config = null)
        {
            var range = (Range) config;
            // Debug.Log($"{gameObject.name}.Apply.Range:{JsonUtility.ToJson(range)}");
            var currentValue = transform.localScale.x;
            // Debug.Log($"{gameObject.name}.Apply.Current Value:{currentValue}");
            var tmp = Mathf.Abs(range.fromValue - range.toValue);
            // Debug.Log($"{gameObject.name}.Apply.Tmp:{tmp}");
            var step = tmp / duration;
            // Debug.Log($"{gameObject.name}.Apply.Step:{step}");
            var journey = Mathf.Abs(currentValue - range.toValue);
            // Debug.Log($"{gameObject.name}.Apply.Journey:{journey}");

            startTransformTime = Time.time;
            complete = false;

            if (journey > 0f)
            {
                var adjustedDuration = step * journey;
                // Debug.Log($"{gameObject.name}.Apply.Adjusted Duration:{adjustedDuration}");

                while (!complete)
                {
                    float fractionComplete =  Mathf.Clamp((Time.time - startTransformTime) / adjustedDuration, 0f, 1f);
                    // Debug.Log($"{gameObject.name}.Apply.Fraction Complete:{fractionComplete}");
                    OnApply(Mathf.Lerp(currentValue, range.toValue, (float) fractionComplete));

                    if (fractionComplete >= 1f)
                    {
                        complete = true;
                    }

                    yield return null;
                }
            }
        }

        protected override void OnApply(object arg1, object arg2 = null)
        {
            var scale = (float) arg1;
            // Debug.Log($"{gameObject.name}.Apply.OnApply:Scale XY : {scale} Z : {transform.localScale.z}");
            transform.localScale = new Vector3(scale, scale, transform.localScale.z);
        }
    }
}