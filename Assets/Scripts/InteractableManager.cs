using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : MonoBehaviour, IInteractable
{
    public enum EventType
    {
        OnSelectEntered,
        OnSelectExited
    }

    public delegate void Event(InteractableManager interactable, EventType type);
    public static event Event EventReceived;
    
    public bool IsHeld { get { return isHeld; } }

    protected XRGrabInteractable interactable;

    private GameObject interactor;
    protected Transform objects;
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
        // Debug.Log($"{Time.time} {gameObject.name}.OnSelectEntered");
        Debug.Log($"{Time.time} {gameObject.name} 1");
        
        // if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        // {
        //     Debug.Log($"{Time.time} {gameObject.name}:Use Gravity: {rigidBody.useGravity}");
        // }

        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(out HandController controller))
        {
            Debug.Log($"{Time.time} {gameObject.name} 2");
            controller.SetHolding(gameObject);
            isHeld = true;
        
            OnSelectEntered(args, controller);
        }

        transform.parent = objects;
    }

    protected virtual void OnSelectEntered(SelectEnterEventArgs args, HandController controller) { }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        // Debug.Log($"{Time.time} {gameObject.name}.OnSelectExited");
        Debug.Log($"{Time.time} {gameObject.name} 3");

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            rigidBody.isKinematic = false;
            // Debug.Log($"{Time.time} {gameObject.name}:Use Gravity: {rigidBody.useGravity} Is Kinematic : {rigidBody.isKinematic}");
            Debug.Log($"{Time.time} {gameObject.name} 4");
        }

        transform.parent = objects;

        if (TryGetController<HandController>(out HandController controller))
        {
            Debug.Log($"{Time.time} {gameObject.name} 5");
            controller.SetHolding(null);
            isHeld = false;
            
            OnSelectExited(args, controller);
        }
        
        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectExited);
        }
        
        interactor = null;
    }

    protected virtual void OnSelectExited(SelectExitEventArgs args, HandController controller) { }

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