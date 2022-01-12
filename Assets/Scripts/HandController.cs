using System;
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
    [SerializeField] InputDevice controller;

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public InputDevice Controller { get { return controller; } }
    public bool IsHolding { get { return isHolding; } }

    private List<InputDevice> controllers;
    private InputDevice thisController;
    private XRController xrController;
    private bool isHolding = false;
    private GameObject holding;
    private DebugCanvas debugCanvas;
    private Gesture lastState;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, controllers);

        if (controllers.Count > 0)
        {
            thisController = controllers[0];
        }

        if (thisController.isValid)
        {
            if (enableLogging) Debug.Log($"{thisController.name}.Detected");

            SetState(Gesture.None);
        }
    }

    private void ResolveDependencies()
    {
        xrController = GetComponent<XRController>() as XRController;

        var obj = FindObjectOfType<DebugCanvas>();
        debugCanvas = obj?.GetComponent<DebugCanvas>() as DebugCanvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (!thisController.isValid) return;

        Gesture thisState = Gesture.None;
        bool hasState = false;

        if (thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            if (enableLogging) Debug.Log($"{thisController.name}.Trigger:{triggerValue}");
            
            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Trigger", triggerValue);

            if (triggerValue > 0.1f)
            {
                thisState |= Gesture.Trigger;
                hasState = true;
            }
        }

        if (thisController.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            if (enableLogging) Debug.Log($"{thisController.name}.Grip:{gripValue}");

            var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
            handAnimationController?.SetFloat("Grip", gripValue);

            if (gripValue > 0.1f)
            {
                thisState |= Gesture.Grip;
                hasState = true;
            }
        }

        if (thisController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbStickValue))
        {
            if (enableLogging) Debug.Log($"{thisController.name}.Thumb Stick:{thumbStickValue.x} {thumbStickValue.y}");

            if (thumbStickValue.x < -0.9f)
            {
                thisState |= Gesture.ThumbStick_Left;
                holding?.GetComponent<GunInteractableManager>().OnGesture(Gesture.ThumbStick_Left);
                hasState = true;
            }
            else if (thumbStickValue.x > 0.9f)
            {
                thisState |= Gesture.ThumbStick_Right;
                holding?.GetComponent<GunInteractableManager>().OnGesture(Gesture.ThumbStick_Right);
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
        if (enableLogging) Debug.Log($"{thisController.name}.State : {state}");
        this.lastState = state;
    }

    public void SetHolding(GameObject obj)
    {
        isHolding = (obj != null);
        holding = obj;

        if (enableLogging) Debug.Log($"{gameObject.name}.IsHolding : {isHolding}");
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
}