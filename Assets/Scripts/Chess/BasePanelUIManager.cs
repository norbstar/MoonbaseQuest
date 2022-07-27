using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

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

        [Header("Config")]
        [SerializeField] bool enableHaptics = false;
        public bool EnableHaptics { get { return enableHaptics; } }

        protected GameObject selectedButton;

        private ScaleFXManager scaleFXManager;

        public virtual void Start() => ResolveDependencies();

        private void ResolveDependencies()
        {
            scaleFXManager = GetComponent<ScaleFXManager>() as ScaleFXManager;
        }

        public void ResetButtons()
        {
            for (int itr = 0; itr < buttons.transform.childCount; itr++)
            {
                var transform = buttons.transform.GetChild(itr);
                transform.localScale = new Vector3(1f, 1f, 1f);

                var manager = transform.gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

                if (manager.Header != null)
                {
                    manager.HeaderColor = manager.DefaultHeaderColor;
                }
            }
        }

        private XRUIInputModule GetXRInputModule() => EventSystem.current.currentInputModule as XRUIInputModule;

        private bool TryGetXRRayInteractor(int pointerID, out XRRayInteractor rayInteractor)
        {
            var inputModule = GetXRInputModule();

            if (inputModule == null)
            {
                rayInteractor = null;
                return false;
            }
    
            rayInteractor = inputModule.GetInteractor(pointerID) as XRRayInteractor;
            return (rayInteractor != null);
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var pointerEventData = ((PointerEventData) eventData);
            
            XRRayInteractor rayInteractor = null;
            
            if (TryGetXRRayInteractor(pointerEventData.pointerId, out var interactor))
            {
                rayInteractor = interactor;
            }

            var gameObject = pointerEventData.pointerEnter;
            selectedButton = gameObject;

            StartCoroutine(OnPointerEnterCoroutine(gameObject, rayInteractor));
        }

        private IEnumerator OnPointerEnterCoroutine(GameObject gameObject, XRRayInteractor rayInteractor)
        {   
            var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.HeaderHighlightColor;
            }

            if (enableHaptics)
            {
                rayInteractor?.SendHapticImpulse(0.25f, 0.1f);
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
            selectedButton = null;
            
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            StartCoroutine(OnPointerExitCoroutine(gameObject));
        }

        private IEnumerator OnPointerExitCoroutine(GameObject gameObject)
        {
            var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.DefaultHeaderColor;
            }

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(1.1f, 1f);
        }

        public virtual void OnClickButton()
        {
            if (onButtonSelectClip == null) return;
            
            AudioSource.PlayClipAtPoint(onButtonSelectClip, Vector3.zero, 1.0f);
        }
    }
}