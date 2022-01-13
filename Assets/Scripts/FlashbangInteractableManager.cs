using System.Collections;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashbangInteractableManager : FocusManager
{
    [SerializeField] IdentityCanvasManager identityCanvasManager;

    [Header("Trigger")]
    [SerializeField] GameObject pin;

    [Header("Timings")]
    [SerializeField] float fuseDelay = 3f;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip detinationClip;

    // [Header("Physics")]
    // [SerializeField] float force = 100f;

    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject interactor;
    // private Rigidbody rigidBody;
    private bool isHeld, released;

    void Awake()
    {
        ResolveDependencies();

        identityCanvasManager.IdentityText = gameObject.name;
    }

    private void ResolveDependencies()
    {
        // rigidBody = GetComponent<Rigidbody>() as Rigidbody;
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

    public void OnActivated(ActivateEventArgs args)
    {
        if (released) return;
        
        // Debug.Log($"{gameObject.name}:On Activated");

        pin.SetActive(false);
        AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);
        // rigidBody.isKinematic = false;
        // rigidBody.useGravity = true;
        released = true;
    }

    public void OnDeactivated(DeactivateEventArgs args)
    {
        // Debug.Log($"{gameObject.name}:On Deactivated");
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

        if (released)
        {
            // rigidBody.AddForce(transform.forward * force);
            StartCoroutine(ActuateCoroutine());
        }
    }

    private IEnumerator ActuateCoroutine()
    {
        yield return new WaitForSeconds(fuseDelay);

        AudioSource.PlayClipAtPoint(detinationClip, transform.position, 1.0f);
        Destroy(gameObject);
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
}