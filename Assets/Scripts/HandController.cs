using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class HandController : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    public static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    public delegate void ActuationEvent(Actuation actuation, InputDeviceCharacteristics characteristics);
    public static event ActuationEvent ActuationEventReceived;

    public delegate void ThumbstickRawEvent(Vector2 thumbstickValue, InputDeviceCharacteristics characteristics);
    public static event ThumbstickRawEvent ThumbstickRawEventReceived;

    public delegate void StateEvent(HandStateCanvasManager.State state, InputDeviceCharacteristics characteristics);
    public static event StateEvent StateEventReceived;

    [Flags]
    public enum Actuation
    {
        None = 0,
        Trigger = 1,
        Grip = 2,
        Button_AX = 4,
        Button_BY = 8,
        Thumbstick_Left = 16,
        Thumbstick_Right = 32,
        Thumbstick_Up = 64,
        Thumbstick_Down = 128,
        Thumbstick_Click = 256,
        Menu_Oculus = 512
    }

    public enum State
    {
        Hovering,
        Holding
    }

    [Header("Mapping")]
    [SerializeField] InputDeviceCharacteristics characteristics;

    [Header("Camera")]
    [SerializeField] new Camera camera;
    
    [Header("Teleport")]
    [SerializeField] bool enableTeleport = true;
    [SerializeField] float teleportSpeed = 5f;

    [Header("Inputs")]
    [SerializeField] float triggerThreshold = 0.1f;
    [SerializeField] float gripThreshold = 0.5f;
    [SerializeField] Vector2 thumbstickThreshold = new Vector2(0.1f, 0.1f);

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public bool IsHolding { get { return isHolding; } }
    public IInteractable Interactable { get { return interactable; } }

    private MainCameraManager cameraManager;
    private InputDevice controller;
    private XRController xrController;
    private bool isHovering = false;
    private bool isHolding = false;
    private IInteractable interactable;
    private DebugCanvas debugCanvas;
    private Actuation actuation, lastActuation;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        xrController = GetComponent<XRController>() as XRController;
        cameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;

        var obj = FindObjectOfType<DebugCanvas>();
        debugCanvas = obj?.GetComponent<DebugCanvas>() as DebugCanvas;
    }

    private void ResolveController()
    {
        List<InputDevice> controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, controllers);

        if (controllers.Count > 0)
        {
            controller = controllers[0];
        }

        if (controller.isValid)
        {
            Log($"{Time.time} {gameObject.name} {className} ResolveController:{controller.name}.Detected");

            SetActuation(Actuation.None);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isValid)
        {
            ResolveController();
            if (!controller.isValid) return;
        }

        lastActuation = actuation;

        var gameObject = interactable?.GetGameObject();
        Vector2 thumbstickValue;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Trigger.Value:{triggerValue}");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue >= triggerThreshold)
            {
                if (!actuation.HasFlag(Actuation.Trigger))
                {
                    actuation |= Actuation.Trigger;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Trigger);
                }
            }
            else
            {
                actuation &= ~Actuation.Trigger;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Grip.Value:{gripValue}");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Grip", gripValue);

            if (gripValue >= gripThreshold)
            {
                if (enableTeleport && (!isHovering) && (!actuation.HasFlag(Actuation.Grip)) && (cameraManager.TryGetObjectHit(out GameObject obj)))
                {
                    if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        StartCoroutine(TeleportGrabbable(obj));
                    }
                }

                if (!actuation.HasFlag(Actuation.Grip))
                {
                    actuation |= Actuation.Grip;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Grip);
                }
            }
            else
            {
                actuation &= ~Actuation.Grip;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonAXValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.AXButton.Pressed:{buttonAXValue}");

            if (buttonAXValue)
            {
                if (!actuation.HasFlag(Actuation.Button_AX))
                {
                    actuation |= Actuation.Button_AX;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Button_AX);
                }
            }
            else
            {
                actuation &= ~Actuation.Button_AX;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out bool buttonBYValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.BYButton.Pressed:{buttonBYValue}");

            if (buttonBYValue)
            {
                if (!actuation.HasFlag(Actuation.Button_BY))
                {
                    actuation |= Actuation.Button_BY;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Button_BY);
                }
            }
            else
            {
                actuation &= ~Actuation.Button_BY;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Thumbstick.Value:{thumbstickValue}");

            if (thumbstickValue.x <= -thumbstickThreshold.x)
            {
                if (!actuation.HasFlag(Actuation.Thumbstick_Left))
                {
                    actuation |= Actuation.Thumbstick_Left;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Thumbstick_Left);
                }
            }
            else
            {
                actuation &= ~Actuation.Thumbstick_Left;
            }
            
            if (thumbstickValue.x > thumbstickThreshold.x)
            {
                if (!actuation.HasFlag(Actuation.Thumbstick_Right))
                {
                    actuation |= Actuation.Thumbstick_Right;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Thumbstick_Right);
                }
            }
            else
            {
                actuation &= ~Actuation.Thumbstick_Right;
            }

            if (thumbstickValue.y <= -thumbstickThreshold.y)
            {
                if (!actuation.HasFlag(Actuation.Thumbstick_Up))
                {
                    actuation |= Actuation.Thumbstick_Up;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Thumbstick_Up);
                }
            }
            else
            {
                actuation &= ~Actuation.Thumbstick_Up;
            }
            
            if (thumbstickValue.y > thumbstickThreshold.y)
            {
                if (!actuation.HasFlag(Actuation.Thumbstick_Down))
                {
                    actuation |= Actuation.Thumbstick_Down;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Thumbstick_Down);
                }
            }
            else
            {
                actuation &= ~Actuation.Thumbstick_Down;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool thumbstickClickValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Thumbstick Click.Pressed:{thumbstickClickValue}");

            if (thumbstickClickValue)
            {
                if (!actuation.HasFlag(Actuation.Thumbstick_Click))
                {
                    actuation |= Actuation.Thumbstick_Click;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Thumbstick_Click);
                }
            }
            else
            {
                actuation &= ~Actuation.Thumbstick_Click;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.MenuButton.Pressed:{menuButtonValue}");

            if (menuButtonValue)
            {
                if (!actuation.HasFlag(Actuation.Menu_Oculus))
                {
                    actuation |= Actuation.Menu_Oculus;
                    // gameObject?.GetComponent<IActuation>()?.OnActuation(Actuation.Menu_Oculus);
                }
            }
            else
            {
                actuation &= ~Actuation.Menu_Oculus;
            }
        }

        if (actuation != lastActuation)
        {
            gameObject?.GetComponent<IActuation>()?.OnActuation(actuation);

            if (ActuationEventReceived != null)
            {
                ActuationEventReceived.Invoke(actuation, characteristics);
            }
        }
        
        if (ThumbstickRawEventReceived != null)
        {
            ThumbstickRawEventReceived.Invoke(thumbstickValue, characteristics);
        }
        
        UpdateState();
    }

    private void UpdateState()
    {
        HandStateCanvasManager.State state = HandStateCanvasManager.State.None;

        if (isHovering)
        {
            state |= HandStateCanvasManager.State.Hover;
        }
        else
        {
            state &= ~HandStateCanvasManager.State.Hover;
        }

        if (isHolding)
        {
            state |= HandStateCanvasManager.State.Grip;
        }
        else
        {
            state &= ~HandStateCanvasManager.State.Grip;
        }

        if (StateEventReceived != null)
        {
            StateEventReceived.Invoke(state, characteristics);
        }
    }

    private void SetActuation(Actuation actuation)
    {
        this.actuation = actuation;
    }

    public void SetHovering(IInteractable interactable, bool isHovering)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHovering:Game Object : {interactable.GetGameObject().name} Is Hovering : {isHovering}");

        this.isHovering = isHovering;
        NotifyOpposingController(State.Hovering, isHovering, interactable);
    }

    public void SetHolding(IInteractable interactable, bool isHolding)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHolding:Game Object : {interactable.GetGameObject().name} Is Holding : {isHolding}");
 
        this.isHolding = isHolding;
        this.interactable = interactable;
        NotifyOpposingController(State.Holding, isHolding, interactable);
    }

    public void OnOpposingEvent(State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {gameObject.name} {className}.OnOpposingEvent:State : {state} GameObject : {obj.GetGameObject().name}");

        interactable?.OnOpposingEvent(state, isTrue, obj);
    }

    private void NotifyOpposingController(State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {gameObject.name} {className}.NotifyOpposingConroller:State : {state} IsTrue : {isTrue} GameObject : {obj.GetGameObject().name}");

        if (TryGet.TryGetOppositeController(this, out HandController opposingController))
        {
            opposingController.OnOpposingEvent(state, isTrue, obj);
        }
    }

    public void SetImpulse(float amplitude = 1.0f, float duration = 0.1f, uint channel = 0)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetImpulse:Amplitude : {amplitude} Duration : {duration} Channel : {channel}");

        UnityEngine.XR.HapticCapabilities capabilities;
        InputDevice device = GetInputDevice();

        if (device.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }

    public InputDevice GetInputDevice()
    {
        var controller = GetComponent<XRController>() as XRController;
        return controller.inputDevice;
    }

    private IEnumerator TeleportGrabbable(GameObject grabbable)
    {
        Log($"{Time.time} {gameObject.name} {className}.TeleportGrabbable:Amplitude : {grabbable.name}");

        float step =  teleportSpeed * Time.deltaTime;

        while (Vector3.Distance(transform.position, grabbable.transform.position) > 0.1f)
        {
            grabbable.transform.position = Vector3.MoveTowards(grabbable.transform.position, transform.position, step);
            yield return null;
        }
    }
}