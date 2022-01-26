using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class HandController : MonoBehaviour
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

    [SerializeField] InputDeviceCharacteristics characteristics;
    [SerializeField] new Camera camera;
    
    [Header("Teleport")]
    [SerializeField] bool enableTeleport = true;
    [SerializeField] float teleportSpeed = 5f;

    [Header("Inputs")]
    [SerializeField] float triggerThreshold = 0.9f;
    [SerializeField] float gripThreshold = 0.9f;
    [SerializeField] Vector2 thumbStickThreshold = new Vector2(0.9f, 0.9f);

    [Header("Stats")]
    [SerializeField] private bool isHovering = false;
    [SerializeField] private bool isHolding = false;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public bool IsHolding { get { return isHolding; } }
    public GameObject Interactable { get { return interactable; } }

    private MainCameraManager cameraManager;
    private InputDevice controller;
    private XRController xrController;
    // private bool isHovering = false;
    // private bool isHolding = false;
    private GameObject interactable;
    private DebugCanvas debugCanvas;
    private Gesture gesture, lastGesture;
    private IController controllerCanvasManager;
    private TestCaseRunner testCaseRunner;

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

        testCaseRunner = TestCaseRunner.GetInstance();
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
            Log($"{gameObject.name} {className} ResolveController:{controller.name}.Detected");

            SetGesture(Gesture.None);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Log($"{gameObject.name} {className} State:Hovering {isHovering} Holding {isHolding}");
        
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
                    if (obj.TryGetComponent<InteractableManager>(out InteractableManager interactable))
                    {
                        Log($"{gameObject.name} {className}.Grip.Teleport:{obj.name}");
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
            Log($"{gameObject.name} {className}.AXButton.Pressed:{buttonAXValue}");

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
            Log($"{gameObject.name} {className}.BYButton.Pressed:{buttonBYValue}");

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
            if (thumbStickValue.x <= -thumbStickThreshold.x)
            {
                gesture |= Gesture.ThumbStick_Left;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Left);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Left;
            }
            
            if (thumbStickValue.x > thumbStickThreshold.x)
            {
                gesture |= Gesture.ThumbStick_Right;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Right);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Right;
            }

            if (thumbStickValue.y <= -thumbStickThreshold.y)
            {
                gesture |= Gesture.ThumbStick_Up;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Up);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Up;
            }
            
            if (thumbStickValue.y > thumbStickThreshold.y)
            {
                gesture |= Gesture.ThumbStick_Down;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Down);
            }
            else
            {
                gesture &= ~Gesture.ThumbStick_Down;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonValue))
        {
            Log($"{gameObject.name} {className}.MenuButton.Pressed:{menuButtonValue}");

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

        if (gesture != lastGesture) Log($"{gameObject.name} {className}.State:{gesture}");
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

    public void SetHovering(GameObject obj)
    {
        isHovering = (obj != null);
        interactable = obj;
    }

    public void SetHolding(GameObject obj)
    {
        isHolding = (obj != null);
        interactable = obj;
    }

    public void SetImpulse(float amplitude = 1.0f, float duration = 0.1f, uint channel = 0)
    {
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
        float step =  teleportSpeed * Time.deltaTime;

        while (Vector3.Distance(transform.position, grabbable.transform.position) > 0.1f)
        {
            grabbable.transform.position = Vector3.MoveTowards(grabbable.transform.position, transform.position, step);
            yield return null;
        }
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}