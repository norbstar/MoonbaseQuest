using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(FX.ScaleFX))]
public class StartButton : FocusManager, IInteractableEvent
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
    // private bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        scaleFX = GetComponent<FX.ScaleFX>() as FX.ScaleFX;
    }

    // public void OnTriggerEnter(Collider collider) => HandleOnTrigger(collider.gameObject);

    // public void OnTriggerStay(Collider collider) => HandleOnTrigger(collider.gameObject);

    // private void HandleOnTrigger(GameObject trigger)
    // {
    //     // if (triggered) return;

    //     Console.Log($"{gameObject.name}.HandleOnTrigger:{trigger.name}");

    //     if (trigger.gameObject.CompareTag("Laser"))
    //     {
    //         // Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerEnter");
    //         audioSource.Play();
    //         // Invoke("PostTriggerEvent", 1f);
    //     }

    //     // triggered = true;
    // }

    protected override void OnFocusGained()
    {
        if (!isActiveAndEnabled) return;
        
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Range
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

        coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Range
        {
            fromValue = 1.2f,
            toValue = 1f
        }));
    }

    public void OnActivate(XRGrabInteractable interactable)
    {
        // Console.Log($"{gameObject.name}.OnActivate:{interactable.name}");

        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);
        EventReceived(Event.OnTrigger);
    }
}