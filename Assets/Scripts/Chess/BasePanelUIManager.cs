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
                var child = buttons.transform.GetChild(itr);
                child.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        public IEnumerator ScaleUpPanelCoroutine()
        {
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var button = ((PointerEventData) eventData).pointerEnter;
            StartCoroutine(ScaleUpButtonCoroutine(button));
        }

        private IEnumerator ScaleUpButtonCoroutine(GameObject button)
        {
            var manager = button.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleUp(1f, 1.1f);
            
            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }

            selectedButton = button;
        }

        public void OnPointerExit(BaseEventData eventData)
        {   
            var button = ((PointerEventData) eventData).pointerEnter;
            StartCoroutine(ScaleDownButtonCoroutine(button));
        }

        private IEnumerator ScaleDownButtonCoroutine(GameObject button)
        {
            var manager = button.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1.1f, 1f);
            
            selectedButton = null;
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