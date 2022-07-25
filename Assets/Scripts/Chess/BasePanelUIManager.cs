using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class BasePanelUIManager : MonoBehaviour/*, IPointerEnterHandler*/
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

        private InputDevice leftDevice, rightDevice;
        private XRController[] controllers;
        private XRController leftController, rightController;
        private ScaleFXManager scaleFXManager;

        public virtual void Start()
        {
            ResolveDependencies();

            foreach (XRController controller in controllers)
            {
                if (controller.name.Equals("Left Hand Controller"))
                {
                    leftController = controller;
                    leftDevice = controller.inputDevice;
                }
                else if (controller.name.Equals("Right Hand Controller"))
                {
                    rightController = controller;
                    rightDevice = controller.inputDevice;
                }
            }
        }

        // public virtual void Start()
        // {
        //     ResolveDependencies();

        //     for (int itr = 0; itr < buttons.transform.childCount; itr++)
        //     {
        //         var child = buttons.transform.GetChild(itr).transform;
        //         var button = child.GetComponent<UnityEngine.UI.Button>() as UnityEngine.UI.Button;
        //         button.onClick.AddListener(() => OnClickButton(button));
        //     }
        // }

        private void ResolveDependencies()
        {
            controllers = FindObjectsOfType<XRController>();
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

        // public void OnPointerEnter(PointerEventData eventData)
        // {
        //     var gameObject = eventData.pointerEnter;
        // }

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
            var gameObject = pointerEventData.pointerEnter;
            // var extendedPointerEventData = ((UnityEngine.InputSystem.UI.ExtendedPointerEventData) eventData);

            // var gameObject = extendedPointerEventData.pointerEnter;
            // var device = extendedPointerEventData.device;
            selectedButton = gameObject;

            if (TryGetXRRayInteractor(pointerEventData.pointerId, out var rayInteractor))
            {
                rayInteractor.SendHapticImpulse(0.5f, 0.1f);
            }

            // if (eventData is TrackedDeviceEventData trackedDeviceEventData)
            // {
            //     if (trackedDeviceEventData.interactor is XRBaseControllerInteractor xrInteractor)
            //     {
            //         xrInteractor.SendHapticImpulse(0.25f, 0.25f);
            //     }
            // }

            // var inputModule = EventSystem.current.currentInputModule as XRUIInputModule;
            // var interactor = inputModule.GetInteractor(pointerEventData.pointerId) as XRRayInteractor;

            // if (interactor != null)
            // {
            //     // interactor.xrController.SendHapticImpulse(0.25f, 0.25f);
            //     var controller = interactor.xrController;
            // }

            StartCoroutine(OnPointerEnterCoroutine(gameObject));
        }

        private IEnumerator OnPointerEnterCoroutine(GameObject gameObject/*, InputDevice device*/)
        {   
            var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;

            if (manager.Header != null)
            {
                manager.HeaderColor = manager.HeaderHighlightColor;
            }

            // if (enableHaptics)
            // {
            //     SetImpulse(device);
            // }
            
            var scaleFXManager = manager.ScaleFXManager;
            yield return scaleFXManager.ScaleUp(1f, 1.1f);
            
            if (onButtonHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onButtonHoverClip, Vector3.zero, 1.0f);
            }
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            selectedButton = null;
            
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

        private void SetImpulse(InputDevice device, float amplitude = 1f, float duration = 0.1f)
        {
            UnityEngine.XR.HapticCapabilities capabilities;

            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }

        private void OnClickButton(UnityEngine.UI.Button button)
        {
            if (onButtonSelectClip != null)
            {
                // Debug.Log($"OnClickButton (.onClick) {button.name}");
                AudioSource.PlayClipAtPoint(onButtonSelectClip, Vector3.zero, 1.0f);
            }
        }

        public virtual void OnClickButton()
        {

            if (onButtonSelectClip != null)
            {
                // Debug.Log($"OnClickButton (event) {selectedButton.name}");
                AudioSource.PlayClipAtPoint(onButtonSelectClip, Vector3.zero, 1.0f);
            }
        }
    }
}