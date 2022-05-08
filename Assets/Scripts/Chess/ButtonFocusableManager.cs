using UnityEngine;
// using UnityEngine.UI;

namespace Chess
{
    public class ButtonFocusableManager : FocusableManager
    {
        // [Header("Components")]
        // [SerializeField] Image backdrop;
        // [SerializeField] Image icon;

        [Header("Config")]
        // [SerializeField] Color inFocusColor;
        // [SerializeField] Vector3 inFocusIconScale = Vector3.one * 1.25f;
        [SerializeField] float inFocusScaleMultiplier = 1.25f;

        private Color defaultColor;
        private Vector3 defaultIconScale;
        private Vector3 defaultScale;

        void Awake()
        {
            // defaultColor = backdrop.color;
            // defaultIconScale = icon.transform.localScale;
            defaultScale = transform.localScale;
        }

        public override void GainedFocus(GameObject gameObject, Vector3 point)
        {
            base.GainedFocus(gameObject, point);
            // backdrop.color = inFocusColor;
            // icon.transform.localScale = inFocusIconScale;
            transform.localScale = defaultScale * inFocusScaleMultiplier;
        }

        public override void LostFocus(GameObject gameObject)
        {
            base.LostFocus(gameObject);
            // backdrop.color = defaultColor;
            // icon.transform.localScale = defaultIconScale;
            transform.localScale = defaultScale;
        }
    }
}