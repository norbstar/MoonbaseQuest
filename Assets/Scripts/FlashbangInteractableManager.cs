using System.Collections;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashbangInteractableManager : FocusManager
{
    public enum State
    {
        Inactive,
        Activated,
        Deployed
    }

    [SerializeField] IdentityCanvasManager identityCanvasManager;

    [Header("Trigger")]
    [SerializeField] GameObject pin;

    [Header("Timings")]
    [SerializeField] float smokeDelay = 3f;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip detinationClip;

    [Header("Particles")]
    [SerializeField] new ParticleSystem particleSystem;

    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private GameObject interactor;
    private bool isHeld;
    private State state;

    void Awake()
    {
        ResolveDependencies();

        identityCanvasManager.IdentityText = gameObject.name;
        particleSystem.Stop();
    }

    private void ResolveDependencies()
    {
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
        if (state == State.Inactive) return;
        
        // Debug.Log($"{gameObject.name}:On Activated");

        pin.SetActive(false);
        AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);
        state = State.Activated;
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

        if (state == State.Activated)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private IEnumerator ActuateCoroutine()
    {
        yield return new WaitForSeconds(smokeDelay);

        AudioSource.PlayClipAtPoint(detinationClip, transform.position, 1.0f);
        particleSystem.transform.parent = null;
        particleSystem.Play();

        state = State.Deployed;
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