using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(FX.ScaleFX))]
public class StartButton : FocusableManager, IInteractableEvent
{
    public enum Event
    {
        OnTrigger
    }

    [SerializeField] AudioClip hitClip;

    public delegate void OnTriggerEvent(Event evt);
    public OnTriggerEvent EventReceived;

    private FX.ScaleFX scaleFX;
    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        scaleFX = GetComponent<FX.ScaleFX>() as FX.ScaleFX;
    }

    protected override void OnFocusGained()
    {
        if (!isActiveAndEnabled) return;
        
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Config
        {
            fromValue = 1f,
            toValue = 1.2f
        }));
    }

    protected override void OnFocusLost()
    {
        if (!isActiveAndEnabled) return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Config
        {
            fromValue = 1.2f,
            toValue = 1f
        }));
    }

    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force)
    {
        // Debug.Log($"{gameObject.name}.OnActivate:{interactable.name} {hitPoint}");

        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);
        EventReceived(Event.OnTrigger);
    }
}