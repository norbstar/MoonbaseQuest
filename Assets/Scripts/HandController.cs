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
        Menu_Oculus = 256
    }

    public enum State
    {
        Hovering,
        Holding
    }

    [SerializeField] InputDeviceCharacteristics characteristics;
    [SerializeField] new Camera camera;
    
    [Header("Teleport")]
    [SerializeField] bool enableTeleport = true;
    [SerializeField] float teleportSpeed = 5f;

    [Header("Inputs")]
    [SerializeField] float triggerThreshold = 0.9f;
    [SerializeField] float gripThreshold = 0.9f;
    [SerializeField] Vector2 thumbStickThreshold = new Vector2(0.9f, 0.9f);

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
        // Log($"{Time.time} {gameObject.name} {className} State:Hovering {isHovering} Holding {isHolding}");
        
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

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue >= triggerThreshold)
            {
                gesture |= Gesture.Trigger;
            }
            else
            {
                gesture &= ~Gesture.Trigger;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
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

                gesture |= Gesture.Grip;
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
                gesture |= Gesture.Button_AX;
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
                gesture |= Gesture.Button_BY;
            }
            else
            {
                gesture &= ~Gesture.Button_BY;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbStickValue))
        {
            var gameObject = interactable?.GetGameObject();
            
            if (thumbStickValue.x <= -thumbStickThreshold.x)
            {
                gesture |= Gesture.ThumbStick_Left;
                gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Left);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Left;
            }
            
            if (thumbStickValue.x > thumbStickThreshold.x)
            {
                gesture |= Gesture.ThumbStick_Right;
                gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Right);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Right;
            }

            if (thumbStickValue.y <= -thumbStickThreshold.y)
            {
                gesture |= Gesture.ThumbStick_Up;
                gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Up);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Up;
            }
            
            if (thumbStickValue.y > thumbStickThreshold.y)
            {
                gesture |= Gesture.ThumbStick_Down;
                gameObject?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Down);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Down;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonValue))
        {
            // Log($"{Time.time} {gameObject.name} {className}.MenuButton.Pressed:{menuButtonValue}");

            if (menuButtonValue)
            {
                gesture |= Gesture.Menu_Oculus;
            }
            else
            {
                gesture &= ~Gesture.Menu_Oculus;
            }
        }

        if (controllerCanvasManager != null)
        {
            controllerCanvasManager.SetGestureState(gesture);
        }

        UpdateHandGestureState();

        if (gesture != lastGesture) Log($"{Time.time} {gameObject.name} {className}.State:{gesture}");
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

        controllerCanvasManager.SetHandGestureState(handGesture);
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

        if (cameraManager.TryGetOppositeHandController(this, out HandController opposingController))
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