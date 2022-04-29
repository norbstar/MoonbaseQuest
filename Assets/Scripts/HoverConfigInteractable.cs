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
        // Debug.Log("OnHoverEntered");

        var interactor = args.interactorObject.transform.gameObject;
        
        if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
        {
            var xrControlller = controller.GetComponent<XRController>() as XRController;
            defaultlSelectUsage = xrControlller.selectUsage;
            xrControlller.selectUsage = selectUsage;

            var device = controller.InputDevice;

            if ((int) device.characteristics == (int) HandController.LeftHand)
            {
                Debug.Log("Apply Left Transform");
                interactable.attachTransform = leftTransform;
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                Debug.Log("Apply Right Transform");
                interactable.attachTransform = rightTransform;
            }
        }
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        // Debug.Log("OnHoverExited");

        var interactor = args.interactorObject.transform.gameObject;
        
        if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
        {
            var xrControlller = controller.GetComponent<XRController>() as XRController;
            xrControlller.selectUsage = defaultlSelectUsage;
        }
    }
}