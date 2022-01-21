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
        ThumbStick_Left = 4,
        ThumbStick_Right = 8,
        ThumbStick_Up = 16,
        ThumbStick_Down = 32
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

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public bool IsHolding { get { return isHolding; } }

    private MainCameraManager cameraManager;
    private InputDevice controller;
    private XRController xrController;
    private bool isHovering = false;
    private bool isHolding = false;
    private GameObject interactable;
    private DebugCanvas debugCanvas;
    private Gesture state;
    private Gesture lastState;
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
            Log($"{Time.time} {gameObject.name} {className} ResolveController:{controller.name}.Detected");

            SetState(Gesture.None);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Log($"{gameObject.name} {className} Status:IsHovering {isHovering} IsHolding {isHolding}");
        
        if (!controller.isValid)
        {
            ResolveController();
            if (!controller.isValid) return;
        }

        lastState = state;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue >= triggerThreshold)
            {
                state |= Gesture.Trigger;
            }
            else
            {
                state &= ~Gesture.Trigger;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Grip", gripValue);

            if (gripValue >= gripThreshold)
            {
                if (enableTeleport && (!isHovering) && (!state.HasFlag(Gesture.Grip)) && (cameraManager.TryGetObjectHit(out GameObject obj)))
                {
                    if (obj.TryGetComponent<InteractableManager>(out InteractableManager interactable))
                    {
                        Log($"{Time.time} {gameObject.name} {className}.Grip.Teleport:{obj.name}");
                        StartCoroutine(TeleportGrabbable(obj));
                    }
                }

                state |= Gesture.Grip;
            }
            else
            {
                state &= ~Gesture.Grip;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbStickValue))
        {
            if (thumbStickValue.x <= -thumbStickThreshold.x)
            {
                state |= Gesture.ThumbStick_Left;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Left);
            }
            else
            {
                state &= ~Gesture.ThumbStick_Left;
            }
            
            if (thumbStickValue.x > thumbStickThreshold.x)
            {
                state |= Gesture.ThumbStick_Right;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Right);
            }
            else
            {
                state &= ~Gesture.ThumbStick_Right;
            }

            if (thumbStickValue.y <= -thumbStickThreshold.y)
            {
                state |= Gesture.ThumbStick_Up;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Up);
            }
            else
            {
                state &= ~Gesture.ThumbStick_Up;
            }
            
            if (thumbStickValue.y > thumbStickThreshold.y)
            {
                state |= Gesture.ThumbStick_Down;
                interactable?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Down);
            }
            else
            {
                state &= ~Gesture.ThumbStick_Down;
            }
        }

        if (state != lastState) Log($"{Time.time} {gameObject.name} {className}.State:{state}");
    }

    private void SetState(Gesture state)
    {
        this.state = state;
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