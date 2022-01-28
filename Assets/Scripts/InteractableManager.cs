using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : MonoBehaviour, IInteractable
{
    public class Cache
    {
        public bool isKinematic;
        public bool useGravity;
    }

    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public enum EventType
    {
        OnSelectEntered,
        OnSelectExited
    }

    [Header("Focus")]
    [SerializeField] FocusableUI focusableUI;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public delegate void Event(InteractableManager interactable, EventType type);
    public static event Event EventReceived;
    
    public bool IsHeld { get { return isHeld; } }
    public bool IsDocked { get { return isDocked; } }
    public List<Collider> Colliders { get { return interactable.colliders; } }

    protected XRGrabInteractable interactable;
    protected Rigidbody rigidBody;
    protected GameObject interactor;
    protected Transform objects;
    protected TestCaseRunner testCaseRunner;

    protected Cache cache;
    private bool isHeld, isDocked;

    protected virtual void Awake()
    {
        ResolveDependencies();

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            cache = new Cache
            {
                isKinematic = rigidBody.isKinematic,
                useGravity = rigidBody.useGravity
            };
        }

        objects = GameObject.Find("Objects").transform;
        interactable.retainTransformParent = false;
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void OnOpposingEvent(HandController.State state, bool isTrue, IInteractable obj) { }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered");

        var interactor = args.interactorObject.transform.gameObject;

        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered:{interactor.name}");

        if (TryGetController<HandController>(interactor, out HandController controller))
        {
            controller.SetHovering(this);
            OnHoverEntered(args, controller);
        }
    }

    protected virtual void OnHoverEntered(HoverEnterEventArgs args, HandController controller) { }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverExited");

        var interactor = args.interactorObject.transform.gameObject;
        
        if (TryGetController<HandController>(interactor, out HandController controller))
        {
            Log($"{Time.time} {gameObject.name} {className} OnHoverExited:{interactor.name}");

            controller.SetHovering(null);
            OnHoverExited(args, controller);
        }
    }

    protected virtual void OnHoverExited(HoverExitEventArgs args, HandController controller) { }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered");

        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(interactor, out HandController controller))
        {
            controller.SetHolding(this);
            isHeld = true;

            OnSelectEntered(args, controller);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectEntered);
        }

        transform.parent = objects;
    }

    protected virtual void OnSelectEntered(SelectEnterEventArgs args, HandController controller) { }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited");

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            rigidBody.isKinematic = cache.isKinematic;
            rigidBody.useGravity = cache.useGravity;
        }

        if (TryGetController<HandController>(interactor, out HandController controller))
        {
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

    protected bool TryGetController<HandController>(GameObject interactor, out HandController controller)
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

    public void OnDockStatusChange(bool isDocked)
    {
        Log($"{Time.time} {gameObject.name} {className} OnDockStatusChange:Is Docked : {isDocked}");

        this.isDocked = isDocked;
        focusableUI.SetActive(!isDocked);
    }

    protected void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}