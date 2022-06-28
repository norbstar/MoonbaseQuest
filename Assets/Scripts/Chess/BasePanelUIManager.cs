using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class BasePanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] GameObject buttons;

        [Header("Manager")]
        [SerializeField] protected CanvasUIManager canvasManager;

        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;
        [SerializeField] protected AudioClip onSelectClip;

        protected GameObject selectedButton;

        private ScaleFXManager scaleFXManager;

        void Start() => ResolveDependencies();

        private void ResolveDependencies() => scaleFXManager = GetComponent<ScaleFXManager>() as ScaleFXManager;

        public void ResetButtons()
        {
            for (int itr = 0; itr < buttons.transform.childCount; itr++)
            {
                var button = buttons.transform.GetChild(itr);
                button.transform.localScale = new Vector3(1f, 1f, 1f);

                var manager = button.GetComponent<ButtonUIManager>() as ButtonUIManager;
                manager.HeaderColor = manager.HeaderDefaultColor;
            }
        }

        public IEnumerator ScaleUpPanelCoroutine()
        {
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var button = ((PointerEventData) eventData).pointerEnter;
            selectedButton = button;
            
            StartCoroutine(OnPointerEnterCoroutine(button));
        }

        private IEnumerator OnPointerEnterCoroutine(GameObject button)
        {   
            var manager = button.GetComponent<ButtonUIManager>() as ButtonUIManager;
            manager.HeaderColor = manager.HeaderHighlightColor;
            
            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
            
            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var button = ((PointerEventData) eventData).pointerEnter;
            selectedButton = null;
            
            StartCoroutine(OnPointerExitCoroutine(button));
        }

        private IEnumerator OnPointerExitCoroutine(GameObject button)
        {
            var manager = button.GetComponent<ButtonUIManager>() as ButtonUIManager;
            manager.HeaderColor = manager.HeaderDefaultColor;

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(1.1f, 1f);
        }

        public virtual void OnClickButton()
        {
            if (onSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
            }
        }
    }
}