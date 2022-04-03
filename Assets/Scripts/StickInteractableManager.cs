using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
public abstract class StickInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
    [Header("Config")]
    [SerializeField] FlightPlatformManager platformManager;
    protected FlightPlatformManager FlightPlatformManager { get { return platformManager; } }

    [SerializeField] HandAnimationController hand;
    // [SerializeField] NavId controllerId;

    public delegate void StickEvent(/*NavId controllerId, */EventType eventType);
    public static event StickEvent StickEventReceived;

    private GameObject xrOrigin;
    

    // Start is called before the first frame update
    void Start()
    {
        hand?.SetFloat("Grip", 1f);
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered:GameObject : {interactor.name}");
        StickEventReceived?.Invoke(/*controllerId, */EventType.OnSelectEntered);
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited:GameObject : {interactor.name}");
        StickEventReceived?.Invoke(/*controllerId, */EventType.OnSelectExited);
    }

    public abstract void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null);
}