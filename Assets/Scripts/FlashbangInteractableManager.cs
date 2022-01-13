using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashbangInteractableManager : FocusManager, IGesture
{
    [SerializeField] IdentityCanvasManager identityCanvasManager;

    [Header("Trigger")]
    [SerializeField] GameObject pin;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;

    // [Header("Physics")]
    // [SerializeField] float force = 100f;

    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject interactor;
    private Rigidbody rigidBody;
    private bool isHeld, released;

    void Awake()
    {
        ResolveDependencies();

        identityCanvasManager.IdentityText = gameObject.name;
    }

    private void ResolveDependencies()
    {
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
    }

    protected override void OnFocusGained()
    {
        identityCanvasManager.gameObject.SetActive(true);
    }

    protected override void OnFocusLost()
    {
        identityCanvasManager.gameObject.SetActive(false);
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Debug.Log($"{gameObject.name}:On Select Entered");
        interactor = args.interactorObject.transform.gameObject;

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
            isHeld = true;
        }
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        // Debug.Log($"{gameObject.name}:On Select Exited");

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(null);
            isHeld = false;
        }

        interactor = null;

        // if (released)
        // {
        //     rigidBody.AddForce(transform.forward * force);
        // }
    }

    private bool TryGetController<HandController>(out HandController controller)
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

    public void OnGesture(HandController.Gesture gesture, object value = null)
    {
        if (released) return;

        // Debug.Log($"{gameObject.name} On Gesture:Gesture : {gesture}");

        switch (gesture)
        {
            case HandController.Gesture.ThumbStick_Left:
            case HandController.Gesture.ThumbStick_Right:
                pin.SetActive(false);
                AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);
                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
                released = true;
                break;
        }
    }
}