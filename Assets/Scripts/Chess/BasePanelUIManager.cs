using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class BasePanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected GameObject buttons;

        [Header("Manager")]
        [SerializeField] protected CanvasUIManager canvasManager;

        [Header("Audio")]
        [SerializeField] protected AudioClip onButtonHoverClip;
        [SerializeField] protected AudioClip onButtonSelectClip;
        [SerializeField] protected AudioClip onToggleSelectClip;

        protected GameObject selectedButton;

        private ScaleFXManager scaleFXManager;

        public virtual void Start() => ResolveDependencies();

        private void ResolveDependencies() => scaleFXManager = GetComponent<ScaleFXManager>() as ScaleFXManager;

        public void ResetButtons()
        {
            for (int itr = 0; itr < buttons.transform.childCount; itr++)
            {
                var button = buttons.transform.GetChild(itr);
                button.transform.localScale = new Vector3(1f, 1f, 1f);

                var manager = button.GetComponent<ButtonUIManager>() as ButtonUIManager;

                if (manager.Header != null)
                {
                    manager.HeaderColor = manager.DefaultHeaderColor;
                }
            }
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

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.HeaderHighlightColor;
            }
            
            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
            
            if (onButtonHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onButtonHoverClip, Vector3.zero, 1.0f);
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

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.DefaultHeaderColor;
            }

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(1.1f, 1f);
        }

        public virtual void OnClickButton()
        {
            if (onButtonSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onButtonSelectClip, Vector3.zero, 1.0f);
            }
        }
    }
}