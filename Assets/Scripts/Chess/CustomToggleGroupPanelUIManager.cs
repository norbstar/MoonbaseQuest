using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Chess
{
    public class CustomToggleGroupPanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected List<Toggle> toggles;
        
        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;
        [SerializeField] protected AudioClip onToggleClip;

        [Header("Config")]
        [SerializeField] bool enableHaptics = false;
        public bool EnableHaptics { get { return enableHaptics; } }
        [SerializeField] float postAnnotationDelay = 0.5f;
        public float PostAnnotationDelay { get { return postAnnotationDelay; } }
        
        [Header("Posts")]
        [SerializeField] List<TextReceiver> receivers;

        private Coroutine postAnnotationCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            foreach (Toggle toggle in toggles)
            {
                toggle.onValueChanged.AddListener(delegate {
                    OnToggle(toggle);
                });
            }
        }

        void OnEnable()
        {
            foreach (Toggle toggle in toggles)
            {
                var eventHandler = toggle.GetComponent<PointerEventHandler>() as PointerEventHandler;
                eventHandler.EventReceived += OnPointerEvent;
            }
        }

        void OnDisable()
        {
            foreach (Toggle toggle in toggles)
            {
                var eventHandler = toggle.GetComponent<PointerEventHandler>() as PointerEventHandler;
                eventHandler.EventReceived -= OnPointerEvent;
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

        private void OnPointerEvent(GameObject gameObject, PointerEventHandler.Event evt, PointerEventData eventData)
        {
            switch (evt)
            {
                case PointerEventHandler.Event.Enter:
                    OnPointerEnter(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Exit:
                    OnPointerExit(gameObject, eventData);
                    break;
            }
        }

        private void OnPointerEnter(GameObject gameObject, PointerEventData eventData)
        {
            XRRayInteractor rayInteractor = null;
            
            if (TryGetXRRayInteractor(eventData.pointerId, out var interactor))
            {
                rayInteractor = interactor;
            }

            StartCoroutine(OnPointerEnterCoroutine(eventData, eventData.pointerEnter, rayInteractor));
            postAnnotationCoroutine = StartCoroutine(PostAnnotationCoroutine(eventData.pointerEnter));
        }

        private IEnumerator OnPointerEnterCoroutine(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            var rootResolver = gameObject.GetComponent<RootResolver>() as RootResolver;
            var manager = rootResolver.Root.GetComponent<ToggleUI>() as ToggleUI;

            if (enableHaptics)
            {
                rayInteractor?.SendHapticImpulse(0.25f, 0.1f);
            }

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
            
            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }
        }

        private IEnumerator PostAnnotationCoroutine(GameObject gameObject)
        {
            yield return new WaitForSeconds(postAnnotationDelay);

            var rootResolver = gameObject.GetComponent<RootResolver>() as RootResolver;
            var manager = rootResolver.Root.GetComponent<ToggleUI>() as ToggleUI;
            NotifyReceivers(manager.Annotation.Text);
        }

        private void OnPointerExit(GameObject gameObject, PointerEventData eventData)
        {
            StartCoroutine(OnPointerExitCoroutine(eventData, eventData.pointerEnter));
            
            if (postAnnotationCoroutine != null)
            {
                StopCoroutine(postAnnotationCoroutine);
            }

            NotifyReceivers(string.Empty);
        }

        private IEnumerator OnPointerExitCoroutine(PointerEventData eventData, GameObject gameObject)
        {
            var rootResolver = gameObject.GetComponent<RootResolver>() as RootResolver;
            var manager = rootResolver.Root.GetComponent<ToggleUI>() as ToggleUI;
            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(1.1f, 1f);
        }

        protected void NotifyReceivers(string text)
        {
            foreach (TextReceiver receiver in receivers)
            {
                receiver.OnText(text);
            }
        }

        public virtual void OnToggle(Toggle toggle)
        {
            if (onToggleClip != null)
            {
                AudioSource.PlayClipAtPoint(onToggleClip, Vector3.zero, 1.0f);
            }

            NotifyReceivers(string.Empty);
        }
    }
}