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

        protected GameObject selectedObject;

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
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            // var manager = gameObject.GetComponent<ButtonScaleFXManager>() as ButtonScaleFXManager;
            // manager.ScaleUp(1f, 1.1f);

            // AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);

            // selectedObject = gameObject;

            StartCoroutine(ScaleUpButtonCoroutine(gameObject));
        }

        private IEnumerator ScaleUpButtonCoroutine(GameObject gameObject)
        {
            var manager = gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleUp(1f, 1.1f);
            
            AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);

            selectedObject = gameObject;
        }

        public void OnPointerExit(BaseEventData eventData)
        {   
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            // var manager = gameObject.GetComponent<ButtonScaleFXManager>() as ButtonScaleFXManager;
            // manager.ScaleDown(1.1f, 1f);

            // selectedObject = null;

            StartCoroutine(ScaleDownButtonCoroutine(gameObject));
        }

        private IEnumerator ScaleDownButtonCoroutine(GameObject gameObject)
        {
            var manager = gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1.1f, 1f);
            
            selectedObject = null;
        }

        public virtual void OnClickButton() => AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
    }
}