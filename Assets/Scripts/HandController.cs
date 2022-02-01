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

    // public class EventArgs
    // {
    //     public bool registerLeftHandGestures;
    //     public bool registerRightHandGestures;
    // }

    // public class GestureDataPoint<T>
    // {
    //     public Gesture gesture;
    //     public T data;
    // }

    // public class RawGestureData
    // {
    //     public Vector2 thumbStickValue;
    // }

    public delegate void GestureEvent(Gesture gesture, InputDeviceCharacteristics characteristics);
    // public delegate void GestureEvent(Gesture gesture, RawGestureData rawGestureData, InputDeviceCharacteristics characteristics);
    // public delegate void Event(GestureDataPoint<object> gesture, InputDeviceCharacteristics characteristics);
    // public static event EventHandler<EventArgs> EventReceived;
    public static event GestureEvent GestureEventReceived;

    public delegate void ThumbstickRawEvent(Vector2 thumbStickValue, InputDeviceCharacteristics characteristics);
    public static event ThumbstickRawEvent ThumbstickRawEventReceived;

    [Flags]
    public enum Gesture
    {
        None = 0,
        Trigger = 1,
        Grip = 2,
        Button_AX = 4,
        Button_BY = 8,
        ThumbStick_Left = 16,
        ThumbStick_Right = 32,
        ThumbStick_Up = 64,
        ThumbStick_Down = 128,
        ThumbStick_Click = 256,
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
    [SerializeField] Vector2 thumbStickThreshold = new Vector2(0.1f, 0.1f);

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
    private Gesture gesture, lastGesture;
    private IController controllerCanvasManager;

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

            SetGesture(Gesture.None);
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

        if (characteristics.HasFlag(InputDeviceCharacteristics.Left))
        {
            var controllerManager = GameObject.FindObjectOfType(typeof(LeftControllerCanvasManager));
            
            if (controllerManager != null)
            {
                controllerCanvasManager = (IController) controllerManager;
            }
        }
        else
        {
            var controllerManager = GameObject.FindObjectOfType(typeof(RightControllerCanvasManager));
            
            if (controllerManager != null)
            {
                controllerCanvasManager = (IController) controllerManager;
            }
        }

        lastGesture = gesture;

        var gameObject = interactable?.GetGameObject();
        Vector2 thumbStickValue;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Trigger.Value:{triggerValue}");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue >= triggerThreshold)
            {
                if (!gesture.HasFlag(Gesture.Trigger))
                {
                    gesture |= Gesture.Trigger;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.Trigger);
                }
            }
            else
            {
                gesture &= ~Gesture.Trigger;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Grip.Value:{gripValue}");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Grip", gripValue);

            if (gripValue >= gripThreshold)
            {
                if (enableTeleport && (!isHovering) && (!gesture.HasFlag(Gesture.Grip)) && (cameraManager.TryGetObjectHit(out GameObject obj)))
                {
                    if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        StartCoroutine(TeleportGrabbable(obj));
                    }
                }

                if (!gesture.HasFlag(Gesture.Grip))
                {
                    gesture |= Gesture.Grip;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.Grip);
                }
            }
            else
            {
                gesture &= ~Gesture.Grip;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonAXValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.AXButton.Pressed:{buttonAXValue}");

            if (buttonAXValue)
            {
                if (!gesture.HasFlag(Gesture.Button_AX))
                {
                    gesture |= Gesture.Button_AX;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.Button_AX);
                }
            }
            else
            {
                gesture &= ~Gesture.Button_AX;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out bool buttonBYValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.BYButton.Pressed:{buttonBYValue}");

            if (buttonBYValue)
            {
                if (!gesture.HasFlag(Gesture.Button_BY))
                {
                    gesture |= Gesture.Button_BY;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.Button_BY);
                }
            }
            else
            {
                gesture &= ~Gesture.Button_BY;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbStickValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Thumbstick.Value:{thumbStickValue}");

            if (thumbStickValue.x <= -thumbStickThreshold.x)
            {
                if (!gesture.HasFlag(Gesture.ThumbStick_Left))
                {
                    gesture |= Gesture.ThumbStick_Left;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Left);
                }
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Left;
            }
            
            if (thumbStickValue.x > thumbStickThreshold.x)
            {
                if (!gesture.HasFlag(Gesture.ThumbStick_Right))
                {
                    gesture |= Gesture.ThumbStick_Right;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Right);
                }
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Right;
            }

            if (thumbStickValue.y <= -thumbStickThreshold.y)
            {
                if (!gesture.HasFlag(Gesture.ThumbStick_Up))
                {
                    gesture |= Gesture.ThumbStick_Up;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Up);
                }
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Up;
            }
            
            if (thumbStickValue.y > thumbStickThreshold.y)
            {
                if (!gesture.HasFlag(Gesture.ThumbStick_Down))
                {
                    gesture |= Gesture.ThumbStick_Down;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Down);
                }
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Down;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool thumbstickClickValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.Thumbstick Click.Pressed:{thumbstickClickValue}");

            if (thumbstickClickValue)
            {
                if (!gesture.HasFlag(Gesture.ThumbStick_Click))
                {
                    gesture |= Gesture.ThumbStick_Click;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Click);
                }
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Click;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.MenuButton.Pressed:{menuButtonValue}");

            if (menuButtonValue)
            {
                if (!gesture.HasFlag(Gesture.Menu_Oculus))
                {
                    gesture |= Gesture.Menu_Oculus;
                    gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.Menu_Oculus);
                }
            }
            else
            {
                gesture &= ~Gesture.Menu_Oculus;
            }
        }

        if (gesture != lastGesture)
        {
            controllerCanvasManager?.SetGestureState(gesture);

            if (GestureEventReceived != null)
            {
                // var gestures = Enum.GetValues(typeof(Gesture));

                // foreach (Gesture thisGesture in gestures)
                // {
                //     if (gesture.HasFlag(thisGesture))
                //     {
                //         switch(gesture)
                //         {
                //             // TODO
                //         }
                //     }
                // }

                // foreach (Delegate thisDelegate in EventReceived.GetInvocationList())
                // {
                //     thisDelegate.DynamicInvoke(gesture, characteristics);
                // }

                // RawGestureData rawGestureData = new RawGestureData
                // {
                //     thumbStickValue = thumbStickValue
                // };
                
                GestureEventReceived.Invoke(gesture, characteristics);
            }
        }
        
        if (ThumbstickRawEventReceived != null)
        {
            ThumbstickRawEventReceived.Invoke(thumbStickValue, characteristics);
        }

        controllerCanvasManager?.SetThumbStickCursor(thumbStickValue);
        
        UpdateHandGestureState();
    }

    private void UpdateHandGestureState()
    {
        HandGestureCanvasManager.Gesture handGesture = HandGestureCanvasManager.Gesture.None;

        if (isHovering)
        {
            handGesture |= HandGestureCanvasManager.Gesture.Hover;
        }
        else
        {
            handGesture &= ~HandGestureCanvasManager.Gesture.Hover;
        }

        if (isHolding)
        {
            handGesture |= HandGestureCanvasManager.Gesture.Grip;
        }
        else
        {
            handGesture &= ~HandGestureCanvasManager.Gesture.Grip;
        }

        controllerCanvasManager?.SetHandGestureState(handGesture);
    }

    private void SetGesture(Gesture gesture)
    {
        this.gesture = gesture;
    }

    public void SetHovering(IInteractable obj, bool isHovering)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHovering:Game Object : {obj.GetGameObject().name} Is Hovering : {isHovering}");

        this.isHovering = isHovering;
        NotifyOpposingConroller(State.Hovering, isHovering, obj);
    }

    public void SetHolding(IInteractable obj, bool isHolding)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHolding:Game Object : {obj.GetGameObject().name} Is Holding : {isHolding}");
 
        this.isHolding = isHolding;
        interactable = obj;
        NotifyOpposingConroller(State.Holding, isHolding, obj);
    }

    public void OnOpposingEvent(State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {gameObject.name} {className}.OnOpposingEvent:State : {state} GameObject : {obj.GetGameObject().name}");

        interactable?.OnOpposingEvent(state, isTrue, obj);
    }

    private void NotifyOpposingConroller(State state, bool isTrue, IInteractable obj)
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