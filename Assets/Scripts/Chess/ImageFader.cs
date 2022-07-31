using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    [RequireComponent(typeof(Image))]
    public class ImageFader : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] float fadeInDurationSec = 0.5f;
        [SerializeField] float fadeOutDurationSec = 0.5f;
        [SerializeField] bool destroyOnCompletion = true;

        public enum Direction
        {
            In,
            Out
        }

        private Image image;
        private float fractionComplete;
        public float FractionComplete { get { return fractionComplete; } }

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => image = GetComponent<Image>() as Image;

        public void FadeIn() => StartCoroutine(FadeCoroutine(Direction.In));

        public void FadeOut() => StartCoroutine(FadeCoroutine(Direction.Out));

        private IEnumerator FadeCoroutine(Direction direction)
        {
            bool complete = false;           
            float startTime = Time.time;            
            float endTime = startTime + fadeOutDurationSec;

            Debug.Log("FadeCoroutine Start");

            while (!complete)
            {
                fractionComplete = (Time.time - startTime) / fadeOutDurationSec;
                Debug.Log($"FadeCoroutine FC : {fractionComplete}");
                complete = (fractionComplete >= 1f);
                
                SetColorFromNormalizedValue((direction == Direction.In) ? fractionComplete : 1f - fractionComplete);
                
                yield return null;
            }

            Debug.Log("FadeCoroutine End");

            if (destroyOnCompletion) Destroy(gameObject);
        }

        private void SetColorFromNormalizedValue(float value) => image.color = new Color(image.color.r, image.color.g, image.color.b, value);
    }
}