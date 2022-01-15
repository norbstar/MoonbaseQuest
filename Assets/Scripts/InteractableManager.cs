using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : MonoBehaviour
{
    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject interactor;
    private bool isHeld;

    protected virtual void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
    }

    public virtual void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
            isHeld = true;
        }
    }

    public virtual void OnSelectExited(SelectExitEventArgs args)
    {
        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(null);
            isHeld = false;
        }

        interactor = null;
    }

    protected bool TryGetController<HandController>(out HandController controller)
    {
        if (interactor != null && interactor.CompareTag("Hand"))
        {
            if (interactor.TryGetComponent<HandController>(out HandController handController))
            {
                controller = handController;
                return true;
            }
        }

        controller = default(HandController);
        return false;
    }
}