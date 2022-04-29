using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class HoverConfigInteractable : MonoBehaviour
{
    [Header("Attach Transforms")]
    [SerializeField] Transform leftTransform;
    [SerializeField] Transform rightTransform;

    [Header("Config")]
    [SerializeField] InputHelpers.Button selectUsage;

    public delegate void AttachTransformUpdateEvent(GameObject gameObject);
    public static event AttachTransformUpdateEvent EventReceived;

    private XRGrabInteractable interactable;
    private InputHelpers.Button defaultlSelectUsage;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
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