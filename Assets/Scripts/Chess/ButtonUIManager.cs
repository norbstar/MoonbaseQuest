using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    [RequireComponent(typeof(UnityButton))]
    [RequireComponent(typeof(PointerEventHandler))]
    public class ButtonUIManager : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;
        [SerializeField] protected AudioClip onSelectClip;
        
        [Header("Config")]
        [SerializeField] bool enableHaptics = false;
        public bool EnableHaptics { get { return enableHaptics; } }
        [SerializeField] bool deselectOnSelect = true;
        [SerializeField] float deselectionDelay = 0.25f;
        public float DeselectionDelay { get { return deselectionDelay; } }
        [SerializeField] float postAnnotationDelay = 0.5f;
        public float PostAnnotationDelay { get { return postAnnotationDelay; } }

        [Header("Posts")]
        [SerializeField] List<TextReceiver> receivers;
        
        public enum Event
        {
            OnPointerEnter,
            OnPointerExit,
            OnClick
        }

        public delegate void OnButtonEvent(GameObject gameObject, Event evt);
        public event OnButtonEvent EventReceived;

        private UnityButton button;
        private Coroutine postAnnotationCoroutine;
        private Vector3 defaultScale;

        public virtual void Awake() => ResolveDependencies();

        private void ResolveDependencies() => button = GetComponent<UnityButton>() as UnityButton;

        // Start is called before the first frame update
        void Start()
        {
            defaultScale = transform.localScale;

            button.onClick.AddListener(delegate {
                OnClickButton(button);
            });
        }

        void OnEnable()
        {
            var eventHandler = button.GetComponent<PointerEventHandler>() as PointerEventHandler;
            eventHandler.EventReceived += OnPointerEvent;
        }

        void OnDisable()
        {
            var eventHandler = button.GetComponent<PointerEventHandler>() as PointerEventHandler;
            eventHandler.EventReceived -= OnPointerEvent;
        }

        public void ResetButtons()
        {
            var transform = button.transform;
            transform.localScale = defaultScale;

            var manager = transform.gameObject.GetComponent<ButtonUI>() as ButtonUI;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.DefaultHeaderColor;
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

                case PointerEventHandler.Event.Down:
                    OnPointerDown(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Up:
                    OnPointerUp(gameObject, eventData);
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

            var manager = gameObject.GetComponent<ButtonUI>() as ButtonUI;

            if (manager.OverridePointerEnterEvent)
            {
                OnPointerEnterOverride(eventData, eventData.pointerEnter, rayInteractor);
            }
            else
            {
                StartCoroutine(OnPointerEnterCoroutine(eventData, eventData.pointerEnter, rayInteractor));
                postAnnotationCoroutine = StartCoroutine(PostAnnotationCoroutine(eventData.pointerEnter));
            }

            EventReceived?.Invoke(gameObject, Event.OnPointerEnter);
        }

        protected virtual void OnPointerEnterOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor) { }

        private IEnumerator OnPointerEnterCoroutine(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            var manager = gameObject.GetComponent<ButtonUI>() as ButtonUI;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.HeaderHighlightColor;
            }

            if (enableHaptics)
            {
                rayInteractor?.SendHapticImpulse(0.25f, 0.1f);
            }

            var scaleFXManager = manager.ScaleFXManager;
            transform.localScale = defaultScale * 1.1f;
            yield return scaleFXManager.ScaleUp(defaultScale.z, defaultScale.z * 1.1f);
            
            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }
        }

        private IEnumerator PostAnnotationCoroutine(GameObject gameObject)
        {
            yield return new WaitForSeconds(postAnnotationDelay);

            var manager = gameObject.GetComponent<ButtonUI>() as ButtonUI;
            NotifyReceivers(manager.Annotation.Text);
        }

        private void OnPointerDown(GameObject gameObject, PointerEventData eventData)
        {
            XRRayInteractor rayInteractor = null;
            
            if (TryGetXRRayInteractor(eventData.pointerId, out var interactor))
            {
                rayInteractor = interactor;
            }

            OnPointerDownOverride(eventData, eventData.pointerEnter, rayInteractor);
        }

        protected virtual void OnPointerDownOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor) { }
        
        private void OnPointerUp(GameObject gameObject, PointerEventData eventData)
        {
            XRRayInteractor rayInteractor = null;
            
            if (TryGetXRRayInteractor(eventData.pointerId, out var interactor))
            {
                rayInteractor = interactor;
            }

            OnPointerDownOverride(eventData, eventData.pointerEnter, rayInteractor);
        }

        protected virtual void OnPointerUpOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor) { }

        private void OnPointerExit(GameObject gameObject, PointerEventData eventData)
        {
            var manager = gameObject.GetComponent<ButtonUI>() as ButtonUI;

            if (manager.OverridePointerExitEvent)
            {
                OnPointerExitOverride(eventData, eventData.pointerEnter);
            }
            else
            {
                StartCoroutine(OnPointerExitCoroutine(eventData, eventData.pointerEnter));
                
                if (postAnnotationCoroutine != null)
                {
                    StopCoroutine(postAnnotationCoroutine);
                }

                NotifyReceivers(string.Empty);
            }

            EventReceived?.Invoke(gameObject, Event.OnPointerExit);
        }

        protected virtual void OnPointerExitOverride(PointerEventData eventData, GameObject gameObject) { }

        private IEnumerator OnPointerExitCoroutine(PointerEventData eventData, GameObject gameObject)
        {
            var manager = gameObject.GetComponent<ButtonUI>() as ButtonUI;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.DefaultHeaderColor;
            }

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(defaultScale.z * 1.1f, defaultScale.z);
        }

        protected void NotifyReceivers(string text)
        {
            foreach (TextReceiver receiver in receivers)
            {
                receiver.OnText(text);
            }
        }

        public virtual void OnClickButton(UnityButton button)
        {
            if (onSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
            }

            NotifyReceivers(string.Empty);
            EventReceived?.Invoke(gameObject, Event.OnClick);

            if (deselectOnSelect)
            {
                StartCoroutine(DeselectCoroutine(deselectionDelay));
            }
        }

        private IEnumerator DeselectCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}