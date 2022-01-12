using System.Collections;

using UnityEngine;

namespace FX
{
    public abstract class BaseFX : MonoBehaviour
    {
        [SerializeField] bool autoEnable;

        public abstract IEnumerator Apply(object config = null);
        protected abstract void OnApply(object arg1, object arg2 = null);

        void OnEnable()
        {
            if (autoEnable)
            {
                StartCoroutine(Apply());
            }
        }

        void OnDisable()
        {
            StopCoroutine(Apply());
        }
    }
}