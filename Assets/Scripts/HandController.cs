using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class HandController : MonoBehaviour
{
    [Flags]
    public enum Gesture
    {
        None = 0,
        Trigger = 1,
        Grip = 2,
        ThumbStick_Left = 4,
        ThumbStick_Right = 8
    }


    [SerializeField] bool enableLogging = false;
    [SerializeField] InputDeviceCharacteristics characteristics;
    [SerializeField] new Camera camera;
    
    [Header("Teleport")]
    [SerializeField] bool enableTeleport = true;
    [SerializeField] float teleportSpeed = 5f;
    [SerializeField] float nearDistance = 0.1f;

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public bool IsHolding { get { return isHolding; } }

    private MainCameraManager cameraManager;
    private InputDevice controller;
    private XRController xrController;
    private bool isHolding = false;
    private GameObject holding;
    private DebugCanvas debugCanvas;
    private Gesture lastState;

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
            Log($"{controller.name}.Detected");

            SetState(Gesture.None);
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

        Gesture thisState = Gesture.None;
        bool hasState = false;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            // Log($"{controller.name}.Trigger:{triggerValue}");
            Debug.Log($"{Time.time} {gameObject.name} 1");
            
            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue > nearDistance)
            {
                thisState |= Gesture.Trigger;
                hasState = true;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            // Log($"{controller.name}.Grip:{gripValue}");
            Debug.Log($"{Time.time} {gameObject.name} 2");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Grip", gripValue);

            if (gripValue > 0.1f)
            {
                if (!thisState.HasFlag(Gesture.Grip) && (cameraManager.TryGetObjectHit(out GameObject obj)))
                {
                    if  (enableTeleport && obj.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
                    {
                        // Debug.Log($"{thisController.name}.Grip.Teleport:{obj.name}");
                        Debug.Log($"{Time.time} {gameObject.name} 3");
                        StartCoroutine(TeleportGrabbable(obj));
                    }
                }

                // if (!thisState.HasFlag(Gesture.Grip) && (cameraManager.TryGetFocus(out GameObject focus)))
                // {
                //     if  (focus.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
                //     {
                //         // Debug.Log($"{thisController.name}.Grip.Teleport:{focus.name}");
                //         StartCoroutine(TeleportGrabbable(focus));
                //     }
                // }

                thisState |= Gesture.Grip;
                hasState = true;
            }
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbStickValue))
        {
            // Log($"{controller.name}.Thumb Stick:{thumbStickValue.x} {thumbStickValue.y}");
            Debug.Log($"{Time.time} {gameObject.name} 4");

            if (thumbStickValue.x < -0.9f)
            {
                thisState |= Gesture.ThumbStick_Left;
                holding?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Left);
                hasState = true;
            }
            else if (thumbStickValue.x > 0.9f)
            {
                thisState |= Gesture.ThumbStick_Right;
                holding?.GetComponent<IGesture>()?.OnGesture(Gesture.ThumbStick_Right);
                hasState = true;
            }
        }

        if (hasState)
        {
            SetState(thisState);
        }
        else
        {
            OnNone();
        }
    }

    private void OnNone()
    {
        SetState(Gesture.None);
    }

    private void SetState(Gesture state)
    {
        // Log($"{controller.name}.State : {state}");
        this.lastState = state;
    }

    public void SetHolding(GameObject obj)
    {
        isHolding = (obj != null);
        holding = obj;

        // Log($"{gameObject.name}.IsHolding : {isHolding}");
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
        Debug.Log($"{Time.time} {gameObject.name} 5");
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