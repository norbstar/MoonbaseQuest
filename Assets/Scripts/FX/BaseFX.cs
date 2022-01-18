using System.Collections;

using UnityEngine;

namespace FX
{
    public abstract class BaseFX : MonoBehaviour
    {
        [SerializeField] bool autoEnable;

        public abstract IEnumerator Apply(object config = null);
        protected abstract void OnApply(object arg1, object arg2 = null);

        private Coroutine coroutine;

        void OnEnable()
        {
            if (autoEnable)
            {
                StartCoroutine();
            }
        }

        public void Start() => StartCoroutine();

        private void StartCoroutine()
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(Apply());
            }
        }

        public void Stop() => StopCoroutine();
        void OnDisable() => StopCoroutine();

        private void StopCoroutine()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
}