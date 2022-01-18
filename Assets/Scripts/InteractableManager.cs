using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableManager : MonoBehaviour, IInteractable
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

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
    private TestCaseRunner testCaseRunner;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        interactable.retainTransformParent = false;
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     Debug.Log($"{this.gameObject.name}.Position:{transform.position}");
    // }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectEntered");
        // Debug.Log($"{Time.time} {gameObject.name} 1");
        // testCaseRunner?.Post($"{className} 1");
        testCaseRunner?.Post($"{className} 1");
        
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectEntered:Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");
            testCaseRunner?.Post($"{className} 2 Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");
        }

        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(out HandController controller))
        {
            // Debug.Log($"{Time.time} {gameObject.name} 2");
            Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectEntered:HandController");
            testCaseRunner?.Post($"{className} 3");
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
        // Debug.Log($"{Time.time} {gameObject.name} 3");
        Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectExited");
        testCaseRunner?.Post($"{className} 4");

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            // Debug.Log($"{Time.time} {gameObject.name}:Use Gravity: {rigidBody.useGravity} Is Kinematic : {rigidBody.isKinematic}");
            // Debug.Log($"{Time.time} {gameObject.name} 4");
            Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectExited:Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");
            testCaseRunner?.Post($"{className} 5 Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");
        }

        // transform.parent = objects;

        if (TryGetController<HandController>(out HandController controller))
        {
            // Debug.Log($"{Time.time} {gameObject.name} 5");
            Debug.Log($"{Time.time} {gameObject.name} {className}.OnSelectExited:HandController");
            testCaseRunner?.Post($"{className} 6");
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