using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : MonoBehaviour, IInteractable
{
    public enum EventType
    {
        OnSelectExited
    }

    public delegate void Event(InteractableManager interactable, EventType type);
    public static event Event EventReceived;
    
    public bool IsHeld { get { return isHeld; } }

    protected XRGrabInteractable interactable;

    private GameObject interactor;
    private Transform objects;
    private bool isHeld;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     Debug.Log($"{this.gameObject.name}.Position:{transform.position}");
    // }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.gameObject;

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            Debug.Log($"{Time.time} 3");
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
        }

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
            isHeld = true;
        
            OnSelectEntered(args, controller);
        }

        transform.parent = objects;
    }

    protected virtual void OnSelectEntered(SelectEnterEventArgs args, HandController controller) { }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        // Debug.Log($"{this.gameObject.name}.Position:{transform.position}");

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(null);
            isHeld = false;
            
            OnSelectExited(args, controller);
        }

        transform.parent = objects;
        
        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectExited);
        }
        
        interactor = null;
    }

    protected virtual void OnSelectExited(SelectExitEventArgs args, HandController controller) { }

    // public void OnExitedInsideStickyDock(GameObject trigger)
    // {
    //     Debug.Log($"{this.gameObject.name}.OnExitedInsideStickyDock:[{gameObject.name}]");

    //     gameObject.transform.parent = trigger.transform;
    //     gameObject.transform.localRotation = Quaternion.identity;
    //     gameObject.transform.localPosition = Vector3.zero;
    // }

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