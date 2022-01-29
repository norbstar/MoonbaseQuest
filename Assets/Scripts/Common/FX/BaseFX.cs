using System.Collections;

using UnityEngine;

namespace FX
{
    public abstract class BaseFX : MonoBehaviour
    {
        [SerializeField] bool autoEnable;

        public abstract IEnumerator Apply(object config = null);
        protected abstract void OnApply(object arg1, object arg2 = null);

        public Vector3 OriginalPosition { get { return originalPosition; } }
        public Quaternion OriginalRotation { get { return originalRotation; } }
        public Vector3 OriginalScale { get { return originalScale; } }

        protected Vector3 originalPosition;
        protected Quaternion originalRotation;
        protected Vector3 originalScale;

        private Coroutine coroutine;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
        }

        void OnEnable()
        {
            if (autoEnable)
            {
                StartCoroutine();
            }
        }

        public void Go() => StartCoroutine();  

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

        public void Reset()
        {
            Stop();

            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            transform.localScale = originalScale;
        }
    }
}