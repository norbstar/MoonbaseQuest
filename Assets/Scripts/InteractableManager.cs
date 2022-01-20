using System.Reflection;

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

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    [Header("Origin")]
    [SerializeField] Transform originTransform;

    [Header("Volumes")]
    [SerializeField] GameObject trackingVolume;
    [SerializeField] GameObject dockingVolume;

    public delegate void Event(InteractableManager interactable, EventType type);
    public static event Event EventReceived;
    
    public bool IsHeld { get { return isHeld; } }
    public Transform OriginTransform { get { return originTransform; } }

    protected XRGrabInteractable interactable;

    private GameObject interactor;
    protected Transform objects;
    private Cache cache;
    private bool isHeld;
    private TestCaseRunner testCaseRunner;

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
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered");

        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
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

        if (TryGetController<HandController>(out HandController controller))
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

    public void ShowTrackingVolume() => trackingVolume.SetActive(true);
    
    public void ShowDockingVolume() => dockingVolume.SetActive(true);
    
    public void HideTrackingVolume() => trackingVolume.SetActive(false);
    
    public void HideDockingVolume() => dockingVolume.SetActive(false);

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

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}