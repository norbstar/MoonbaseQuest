using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using Enum;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : GizmoManager, IInteractable
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

    [Header("Optional Settings")]
    [SerializeField] bool enableGravityOnGrab = true;

    public delegate void Event(InteractableManager interactable, EventType type);
    public static event Event EventReceived;
    
    public bool IsHeld { get { return isHeld; } }
    public bool IsDocked { get { return isDocked; } }
    public List<Collider> Colliders { get { return interactable.colliders; } }
    public bool EnableGravityOnGrab { get { return enableGravityOnGrab; } }

    protected XRGrabInteractable interactable;
    protected Rigidbody rigidBody;
    protected GameObject interactor;
    protected Transform objects;

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
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void OnOpposingEvent(Enum.ControllerEnums.State state, bool isTrue, IInteractable obj) { }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        var interactor = args.interactorObject.transform.gameObject;
        // Log($"{Time.time} {gameObject.name} {className} OnHoverEntered:{interactor.name}");

        if (TryGet.TryIdentifyController(interactor, out HandController controller))
        {
            controller.SetHovering(this, true);
            OnHoverEntered(args, controller);
        }
    }

    protected virtual void OnHoverEntered(HoverEnterEventArgs args, HandController controller) { }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        var interactor = args.interactorObject.transform.gameObject;
        // Log($"{Time.time} {gameObject.name} {className} OnHoverExited:{interactor.name}");
        
        if (TryGet.TryIdentifyController(interactor, out HandController controller))
        {
            controller.SetHovering(this, false);
            OnHoverExited(args, controller);
        }
    }

    protected virtual void OnHoverExited(HoverExitEventArgs args, HandController controller) { }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.gameObject;
        // Log($"{Time.time} {gameObject.name} {className} OnSelectEntered:{interactor.name}");

        if (TryGet.TryIdentifyController(interactor, out HandController controller))
        {
            controller.SetHolding(this, true);
            isHeld = true;

            OnSelectEntered(args, controller);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectEntered);
        }

        if (enableGravityOnGrab)
        {
            enableGravityOnGrab = false;
            cache.isKinematic = false;
            cache.useGravity = true;
        }

        transform.parent = objects;
    }

    protected virtual void OnSelectEntered(SelectEnterEventArgs args, HandController controller) { }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        interactor = args.interactorObject.transform.gameObject;
        // Log($"{Time.time} {gameObject.name} {className} OnSelectExited:{interactor.name}");

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            rigidBody.isKinematic = cache.isKinematic;
            rigidBody.useGravity = cache.useGravity;
        }

        if (TryGet.TryIdentifyController(interactor, out HandController controller))
        {
            controller.SetHolding(this, false);
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

    public void OnDockStatusChange(bool isDocked)
    {
        // Log($"{Time.time} {gameObject.name} {className} OnDockStatusChange:Is Docked : {isDocked}");

        this.isDocked = isDocked;
        focusableUI.SetActive(!isDocked);
    }
}