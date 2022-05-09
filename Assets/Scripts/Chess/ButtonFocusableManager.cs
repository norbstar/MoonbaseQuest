using UnityEngine;

namespace Chess
{
    public class ButtonFocusableManager : FocusableManager
    {
        [Header("Config")]
        [SerializeField] float inFocusScaleMultiplier = 1.25f;
        [SerializeField] Vector3 inFocusPositionOffset;

        private Color originalColor;
        private Vector3 originalIconScale;
        private Vector3 originalScale;
        private Vector3 originalPosition;

        void Awake()
        {
            originalScale = transform.localScale;
            originalPosition = transform.localPosition;
        }

        public override void GainedFocus(GameObject gameObject, Vector3 point)
        {
            base.GainedFocus(gameObject, point);
            transform.localScale = originalScale * inFocusScaleMultiplier;
            transform.localPosition += inFocusPositionOffset;
        }

        public override void LostFocus(GameObject gameObject)
        {
            base.LostFocus(gameObject);
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
        }
    }
}