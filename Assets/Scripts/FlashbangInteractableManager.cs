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

    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject lastEngageable;
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