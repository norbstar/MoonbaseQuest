using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class HandController : GizmoManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    public static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    public delegate void InputChangeEvent(HandController controller, Enum.ControllerEnums.Input input, InputDeviceCharacteristics characteristics);
    public static event InputChangeEvent InputChangeEventReceived;

    public class RawInput
    {
        public float triggerValue, gripValue;
        public bool buttonAXValue, buttonBYValue, thumbstickClickValue, menuButtonValue;
        public Vector2 thumbstickValue;
    }

    public delegate void RawInputEvent(RawInput rawData, InputDeviceCharacteristics characteristics);
    public static event RawInputEvent RawInputEventReceived;

    public delegate void ActionEvent(Enum.HandEnums.Action action, InputDeviceCharacteristics characteristics, IInteractable interactable);
    public static event ActionEvent ActionEventReceived;

    // public delegate void HoveringEvent(bool isHovering, IInteractable interactable, InputDeviceCharacteristics characteristics);
    // public static event HoveringEvent HoveringEventReceived;

    // public delegate void HoldingEvent(bool isHolding, IInteractable interactable, InputDeviceCharacteristics characteristics);
    // public static event HoldingEvent HoldingEventReceived;

    [Header("Mapping")]
    [SerializeField] InputDeviceCharacteristics characteristics;

    [Header("Camera")]
    [SerializeField] new Camera camera;

    [Header("Colliders")]
    [SerializeField] protected SphereCollider sphereCollider;

    [Header("Teleport")]
    [SerializeField] bool enableTeleport = true;
    [SerializeField] float teleportSpeed = 5f;

    [Header("Balancing")]
    [SerializeField] float triggerThreshold = 0.1f;
    [SerializeField] float gripThreshold = 0.5f;
    [SerializeField] Vector2 thumbstickThreshold = new Vector2(0.8f, 0.8f);

    public InputDeviceCharacteristics Characteristics { get { return characteristics; } }
    public InputDevice InputDevice { get { return GetComponent<XRController>().inputDevice; } }
    public bool IsHolding { get { return isHolding; } }
    public IInteractable Interactable { get { return interactable; } }

    private TrackingMainCameraManager cameraManager;
    private InputDevice controller;
    private XRController xrController;
    private Transform modelParent;
    private bool isHovering = false;
    private bool isHolding = false;
    private IInteractable interactable;
    private DebugCanvas debugCanvas;
    private Enum.ControllerEnums.Input input, lastInput;

    public virtual void Awake() => ResolveDependencies();

    private void ResolveDependencies()
    {
        xrController = GetComponent<XRController>() as XRController;
        modelParent = xrController.modelParent;

        cameraManager = camera.GetComponent<TrackingMainCameraManager>() as TrackingMainCameraManager;

        var obj = FindObjectOfType<DebugCanvas>();
        debugCanvas = obj?.GetComponent<DebugCanvas>() as DebugCanvas;
    }

    private List<InputFeatureUsage> ResolveFeatures(InputDevice device)
    {
        var inputFeatures = new List<InputFeatureUsage>();
        
        if (device.TryGetFeatureUsages(inputFeatures))
        {
            foreach (var feature in inputFeatures)
            {
                Debug.Log($"Discovered feature : {feature.name}");
                Log($"{Time.time} {gameObject.name} {className} ResolveFeature:{controller.name}-{feature.name}.Detected");
            }
        }

        return inputFeatures;
    }

    private void ResolveController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

        if (devices.Count > 0)
        {
            controller = devices[0];
        }

        if (controller.isValid)
        {
            Log($"{Time.time} {gameObject.name} {className} ResolveController:{controller.name}.Detected");
            SetInput(Enum.ControllerEnums.Input.None);
        }
    }

    public void ShowModel() => modelParent.gameObject.SetActive(true);

    public void HideModel() => modelParent.gameObject.SetActive(false);

    private Enum.ControllerEnums.Input HandleTrigger(float value, Enum.ControllerEnums.Input actuation)
    {
        var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
        handAnimationController?.SetFloat("Trigger", value);

        if (value >= triggerThreshold)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Trigger))
            {
                actuation |= Enum.ControllerEnums.Input.Trigger;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Trigger;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input HandleGrip(float value, Enum.ControllerEnums.Input actuation)
    {
        var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
        handAnimationController?.SetFloat("Grip", value);

        if (value >= gripThreshold)
        {
            if (enableTeleport && (!isHovering) && (!actuation.HasFlag(Enum.ControllerEnums.Input.Grip)) && (cameraManager.TryGetObjectHit(out GameObject obj)))
            {
                if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    StartCoroutine(TeleportGrabbable(obj));
                }
            }

            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Grip))
            {
                actuation |= Enum.ControllerEnums.Input.Grip;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Grip;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input HandleAXButton(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX))
            {
                actuation |= Enum.ControllerEnums.Input.Button_AX;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Button_AX;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input HandleAXTouch(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Touch_AX))
            {
                actuation |= Enum.ControllerEnums.Input.Touch_AX;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Touch_AX;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input HandleBYButton(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Button_BY))
            {
                actuation |= Enum.ControllerEnums.Input.Button_BY;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Button_BY;
        }

        return actuation;
    }

        private Enum.ControllerEnums.Input HandleBYTouch(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Touch_BY))
            {
                actuation |= Enum.ControllerEnums.Input.Touch_BY;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Touch_BY;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input Handle2DAxis(Vector2 value, Enum.ControllerEnums.Input actuation)
    {
        if (value.x <= -thumbstickThreshold.x)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Left))
            {
                actuation |= Enum.ControllerEnums.Input.Thumbstick_Left;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Thumbstick_Left;
        }
        
        if (value.x > thumbstickThreshold.x)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Right))
            {
                actuation |= Enum.ControllerEnums.Input.Thumbstick_Right;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Thumbstick_Right;
        }

        if (value.y > thumbstickThreshold.y)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Up))
            {
                actuation |= Enum.ControllerEnums.Input.Thumbstick_Up;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Thumbstick_Up;
        }
        
        if (value.y <= -thumbstickThreshold.y)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Down))
            {
                actuation |= Enum.ControllerEnums.Input.Thumbstick_Down;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Thumbstick_Down;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input Handle2DAxisClick(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Thumbstick_Click))
            {
                actuation |= Enum.ControllerEnums.Input.Thumbstick_Click;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Thumbstick_Click;
        }

        return actuation;
    }

    private Enum.ControllerEnums.Input HandleMenuButton(bool value, Enum.ControllerEnums.Input actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Menu_Oculus))
            {
                actuation |= Enum.ControllerEnums.Input.Menu_Oculus;
            }
        }
        else
        {
            actuation &= ~Enum.ControllerEnums.Input.Menu_Oculus;
        }

        return actuation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isValid)
        {
            ResolveController();
            if (!controller.isValid) return;
        }

        lastInput = input;

        Log($"{this.gameObject.name} {className}.Update:Interactable : {interactable != null}");

        float triggerValue, gripValue;
        bool buttonAXValue, touchAXValue, buttonBYValue, touchBYValue, thumbstickClickValue, menuButtonValue;
        Vector2 thumbstickValue;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
        {
            input = HandleTrigger(triggerValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out gripValue))
        {
            input = HandleGrip(gripValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out buttonAXValue))
        {
            input = HandleAXButton(buttonAXValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryTouch, out touchAXValue))
        {
            input = HandleAXTouch(touchAXValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonBYValue))
        {
            input = HandleBYButton(buttonBYValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryTouch, out touchBYValue))
        {
            input = HandleBYTouch(touchBYValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickValue))
        {
            input = Handle2DAxis(thumbstickValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out thumbstickClickValue))
        {
            input = Handle2DAxisClick(thumbstickClickValue, input);
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonValue))
        {
            input = HandleMenuButton(menuButtonValue, input);
        }

        PollInputChange();
        PushRawInput(triggerValue, gripValue, buttonAXValue, touchAXValue, buttonBYValue, touchBYValue, thumbstickClickValue, menuButtonValue, thumbstickValue);
        PollIsPinchingOrGripping();
        PushAction();
    }

    protected virtual void IsPinchingOrGripping(bool pinchingOrGripping) { }

    public bool InputContains(Enum.ControllerEnums.Input thisInput) => (input & thisInput) == thisInput;

    private void PollInputChange()
    {
        if (input == lastInput) return;

        var gameObject = interactable?.GetGameObject();
        gameObject?.GetComponent<IInputChange>()?.OnInputChange(input, characteristics);

        // if (gameObject?.TryGetComponent<IInputChange>(out IInputChange callback))
        // {
        //     callback.OnInputChange(actuation, characteristics);
        // }

        if (InputChangeEventReceived != null)
        {
            InputChangeEventReceived.Invoke(this, input, characteristics);
        }
    }

    private void PushRawInput(float triggerValue, float gripValue, bool buttonAXValue, bool touchAXValue, bool buttonBYValue, bool touchBYValue, bool thumbstickClickValue, bool menuButtonValue, Vector2 thumbstickValue)
    {
        if (RawInputEventReceived == null) return;

        var rawInput = new RawInput
        {
            triggerValue = triggerValue,
            gripValue = gripValue,
            buttonAXValue = buttonAXValue,
            buttonBYValue = buttonBYValue,
            thumbstickClickValue = thumbstickClickValue,
            menuButtonValue = menuButtonValue,
            thumbstickValue = thumbstickValue
        };

        var gameObject = interactable?.GetGameObject();
        gameObject?.GetComponent<IRawInput>()?.OnRawInput(rawInput);
        
        // if (gameObject?.TryGetComponent<IRawInput>(out IRawInput callback))
        // {
        //     callback.OnRawInput(rawInput);
        // }

        RawInputEventReceived.Invoke(rawInput, characteristics);
    }

    private void PollIsPinchingOrGripping()
    {
        bool pinching = (InputContains(Enum.ControllerEnums.Input.Trigger));
        bool gripping = (InputContains(Enum.ControllerEnums.Input.Grip));

        IsPinchingOrGripping(pinching | gripping);
    }

    private void PushAction()
    {
        var action = Enum.HandEnums.Action.None;

        if (isHolding)
        {
            action |= Enum.HandEnums.Action.Holding;
        }
        else
        {
            action &= ~Enum.HandEnums.Action.Holding;
        }

        if (isHovering)
        {
            action |= Enum.HandEnums.Action.Hovering;
        }
        else
        {
            action &= ~Enum.HandEnums.Action.Hovering;
        }

        if (ActionEventReceived != null)
        {
            Log($"{gameObject.name} {className} PushAction Action: {action}");
            ActionEventReceived.Invoke(action, characteristics, interactable);
        }
    }

    private void SetInput(Enum.ControllerEnums.Input input) => this.input = input;

    public void SetHovering(IInteractable interactable, bool isHovering)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHovering:Game Object : {interactable.GetGameObject().name} Is Hovering : {isHovering}");

        this.isHovering = isHovering;

        // if (HoveringEventReceived != null)
        // {
        //     HoveringEventReceived.Invoke(isHovering, interactable, characteristics);
        // }

        NotifyOpposingController(Enum.ControllerEnums.State.Hovering, isHovering, interactable);
    }

    public void SetHolding(IInteractable interactable, bool isHolding)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHolding:Game Object : {interactable.GetGameObject().name} Is Holding : {isHolding}");
 
        this.isHolding = isHolding;
        this.interactable = (isHolding) ? interactable : null;

        // if (HoldingEventReceived != null)
        // {
        //     HoldingEventReceived.Invoke(isHolding, interactable, characteristics);
        // }

        NotifyOpposingController(Enum.ControllerEnums.State.Holding, isHolding, interactable);
    }

    public void OnOpposingEvent(Enum.ControllerEnums.State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {gameObject.name} {className}.OnOpposingEvent:State : {state} GameObject : {obj.GetGameObject().name}");

        interactable?.OnOpposingEvent(state, isTrue, obj);
    }

    private void NotifyOpposingController(Enum.ControllerEnums.State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {gameObject.name} {className}.NotifyOpposingConroller:State : {state} IsTrue : {isTrue} GameObject : {obj.GetGameObject().name}");

        if (TryGet.TryGetOpposingController(this, out HandController opposingController))
        {
            opposingController.OnOpposingEvent(state, isTrue, obj);
        }
    }

    public void SetImpulse(float amplitude = 1.0f, float duration = 0.1f, uint channel = 0)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetImpulse:Amplitude : {amplitude} Duration : {duration} Channel : {channel}");

        UnityEngine.XR.HapticCapabilities capabilities;
        InputDevice device = InputDevice;

        if (device.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
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