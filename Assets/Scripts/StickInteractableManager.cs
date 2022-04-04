using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
public abstract class StickInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
    [Header("Components")]
    [SerializeField] GameObject body;

    [Header("Materials")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material interactedMaterial;

    [Header("Config")]
    [SerializeField] FlightPlatformManager platformManager;
    protected FlightPlatformManager FlightPlatformManager { get { return platformManager; } }

    [SerializeField] HandAnimationController hand;
    [SerializeField] NavId controllerId;
    public NavId ControllerId { get  { return controllerId; } }

    public delegate void StickEvent(NavId controllerId, EventType eventType);
    public static event StickEvent StickEventReceived;

    private GameObject xrOrigin;
    private new Renderer renderer;

    protected override void Awake()
    {
        base.Awake();
        
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        renderer = body.GetComponent<Renderer>() as Renderer;
    }

    // Start is called before the first frame update
    void Start()
    {
        hand?.SetFloat("Grip", 1f);
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered:GameObject : {interactor.name}");

        renderer.material = interactedMaterial;
        StickEventReceived?.Invoke(controllerId, EventType.OnSelectEntered);
    }

    public void OnActivated(ActivateEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActivated");
        StickEventReceived?.Invoke(controllerId, EventType.OnActivated);
    }

    public void OnDeactivated(DeactivateEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnDeactivated");
        StickEventReceived?.Invoke(controllerId, EventType.OnDeactivated);
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited:GameObject : {interactor.name}");

        renderer.material = defaultMaterial;
        StickEventReceived?.Invoke(controllerId, EventType.OnSelectExited);
    }

    protected float SignedEulerAngle(float eulerAngle)
    {
        if (eulerAngle >= 180)
        {
            eulerAngle = (360f - eulerAngle) * -1;
        }

        return eulerAngle;
    }

    public abstract void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null);
}