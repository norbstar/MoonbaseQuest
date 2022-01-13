using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]

public class InteractableManager : MonoBehaviour
{
    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject lastEngageable;
    private Rigidbody rigidBody;
    private bool isHeld;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
    }

    public void OnTriggerEnter(Collider collider)
    {
        // Debug.Log($"{gameObject.name}:On Trigger Enter : {collider.gameObject.tag}");
        
        if (collider.gameObject.CompareTag("Hand"))
        {
            lastEngageable = collider.gameObject;
        }
    }

    public void OnSelectEntered()
    {
        // Debug.Log($"{gameObject.name}:On Select Entered");

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
        }
    }

    public void OnSelectExited()
    {
        // Debug.Log($"{gameObject.name}:On Select Exited");

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(null);
        }
    }

    private bool TryGetController<HandController>(out HandController controller)
    {
        if (lastEngageable != null && lastEngageable.CompareTag("Hand"))
        {
            if (lastEngageable.TryGetComponent<HandController>(out HandController handController))
            {
                controller = handController;
                return true;
            }
        }

        controller = default(HandController);
        return false;
    }
}