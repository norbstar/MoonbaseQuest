using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class ShortcutPanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected GameObject group;

        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;
        [SerializeField] protected AudioClip onSelectClip;
        
        [Header("Config")]
        [SerializeField] bool enableHaptics = false;
        public bool EnableHaptics { get { return enableHaptics; } }
        [SerializeField] float postAnnotationDelay = 0.5f;
        public float PostAnnotationDelay { get { return postAnnotationDelay; } }

        [Header("Receivers")]
        [SerializeField] List<TextReceiver> receivers;
        // [SerializeField] DialogPanelUIManager dialogManager;

        public delegate void OnClickEvent(Identity button);
        public static event OnClickEvent EventReceived;

        public enum Identity
        {
            Game,
            Settings,
            About,
            Exit
        }

        private List<UnityButton> buttons;
        private Coroutine postAnnotationCoroutine;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => buttons = group.GetComponentsInChildren<UnityButton>().ToList();

        // Start is called before the first frame update
        void Start()
        {
            foreach (UnityButton button in buttons)
            {
                button.onClick.AddListener(delegate {
                    OnClickButton(button);
                });
            }
        }

        void OnEnable()
        {
            foreach (UnityButton button in buttons)
            {
                var eventHandler = button.GetComponent<PointerEventHandler>() as PointerEventHandler;
                eventHandler.EventReceived += OnPointerEvent;
            }
        }

        void OnDisable()
        {
            foreach (UnityButton button in buttons)
            {
                var eventHandler = button.GetComponent<PointerEventHandler>() as PointerEventHandler;
                eventHandler.EventReceived -= OnPointerEvent;
            }
        }

        public void ResetButtons()
        {
            foreach (UnityButton button in buttons)
            {
                var transform = button.transform;

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

         private void OnPointerEvent(GameObject gameObject, PointerEventHandler.Event evt, PointerEventData eventData)
        {
            // var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

            switch (evt)
            {
                case PointerEventHandler.Event.Enter:
                    // foreach (TextReceiver receiver in receivers)
                    // {
                    //     receiver.OnText(annotation.Text);
                    // }

                    // dialogManager.Text = manager.Annotation.Text;
                    OnPointerEnter(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Exit:
                    // dialogManager.Reset();
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

            // foreach (ITextReceiver receiver in receivers)
            // {
            //     receiver.OnText(eventData.button)
            // }

            StartCoroutine(OnPointerEnterCoroutine(eventData, eventData.pointerEnter, rayInteractor));
            postAnnotationCoroutine = StartCoroutine(PostAnnotationCoroutine(gameObject));
        }

        private IEnumerator OnPointerEnterCoroutine(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
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
            
            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }
        }

        private IEnumerator PostAnnotationCoroutine(GameObject gameObject)
        {
            yield return new WaitForSeconds(postAnnotationDelay);

            var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;
            // dialogManager.Text = manager.Annotation.Text;

            foreach (ITextReceiver receiver in receivers)
            {
                receiver.OnText(manager.Annotation.Text);
            }
        }

        private void OnPointerExit(GameObject gameObject, PointerEventData eventData)
        {
            StartCoroutine(OnPointerExitCoroutine(eventData, eventData.pointerEnter));
            
            if (postAnnotationCoroutine != null)
            {
                StopCoroutine(postAnnotationCoroutine);
            }

            // dialogManager.Reset();

            foreach (ITextReceiver receiver in receivers)
            {
                receiver.OnText(string.Empty);
            }
        }

        private IEnumerator OnPointerExitCoroutine(PointerEventData eventData, GameObject gameObject)
        {
            var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.DefaultHeaderColor;
            }

            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleDown(1.1f, 1f);
        }

        public void OnClickButton(UnityButton button)
        {
            if (onSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
            }

            var name = button.name;

            if (name.Equals("Game Button"))
            {
                EventReceived?.Invoke(Identity.Game);
            }
            else if (name.Equals("Settings Button"))
            {
                EventReceived?.Invoke(Identity.Settings);
            }
            else if (name.Equals("About Button"))
            {
                EventReceived?.Invoke(Identity.About);
            }
            else if (name.Equals("Exit Button"))
            {
                EventReceived?.Invoke(Identity.Exit);
            }
        }
    }
}