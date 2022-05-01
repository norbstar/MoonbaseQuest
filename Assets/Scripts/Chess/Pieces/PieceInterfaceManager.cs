using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chess.Pieces
{
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(PieceManager))]
    public class PieceInterfaceManager : MonoBehaviour, IPieceInteractable
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Attach Transforms")]
        [SerializeField] Transform leftTransform;
        [SerializeField] Transform rightTransform;

        [Header("Config")]
        [SerializeField] InputHelpers.Button selectUsage;

        public delegate void AttachTransformUpdateEvent(GameObject gameObject);
        public static event AttachTransformUpdateEvent EventReceived;

        public bool IsSocketed
        {
            get
            {
                return isSocketed;
            }

            set
            {
                isSocketed = value;
            }    
        }

        private XRGrabInteractable interactable;
        private PieceManager pieceManager;
        private InputHelpers.Button defaultlSelectUsage;
        private bool isSocketed;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
            pieceManager = GetComponent<PieceManager>() as PieceManager;
        }

        public void OnSocketed(bool isSocketed)
        {
            this.isSocketed = isSocketed;
        }
        
        public void OnHoverEntered(HoverEnterEventArgs args)
        {
            var interactor = args.interactorObject.transform.gameObject;
            
            if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
            {
                var xrControlller = controller.GetComponent<XRController>() as XRController;
                defaultlSelectUsage = xrControlller.selectUsage;
                xrControlller.selectUsage = selectUsage;

                var device = controller.InputDevice;

                if ((int) device.characteristics == (int) HandController.LeftHand)
                {
                    interactable.attachTransform = leftTransform;
                }
                else if ((int) device.characteristics == (int) HandController.RightHand)
                {
                    interactable.attachTransform = rightTransform;
                }

                EventReceived?.Invoke(gameObject);
            }
        }

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!isSocketed)
            {
                pieceManager.ShowOutline();
            }
        }

        public void OnSelectExited(SelectExitEventArgs args) => pieceManager.HideOutline();

        public void OnHoverExited(HoverExitEventArgs args)
        {
            var interactor = args.interactorObject.transform.gameObject;
            
            if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
            {
                var xrControlller = controller.GetComponent<XRController>() as XRController;
                xrControlller.selectUsage = defaultlSelectUsage;
            }
        }
    }
}