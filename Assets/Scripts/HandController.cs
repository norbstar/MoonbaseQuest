using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

[RequireComponent(typeof(XRController))]
public class HandController : GizmoManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    public static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    public delegate void ActuationEvent(Actuation actuation, InputDeviceCharacteristics characteristics);
    public static event ActuationEvent ActuationEventReceived;

    public class RawData
    {
        public float triggerValue, gripValue;
        public bool buttonAXValue, buttonBYValue, thumbstickClickValue, menuButtonValue;
        public Vector2 thumbstickValue;
    }

    public delegate void RawDataEvent(RawData rawData, InputDeviceCharacteristics characteristics);
    public static event RawDataEvent RawDataEventReceived;

    public delegate void StateEvent(Enum.HandEnums.State state, InputDeviceCharacteristics characteristics);
    public static event StateEvent StateEventReceived;

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
    private bool isHovering = false;
    private bool isHolding = false;
    private IInteractable interactable;
    private DebugCanvas debugCanvas;
    private Actuation actuation, lastActuation;

    public virtual void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        xrController = GetComponent<XRController>() as XRController;
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
            SetActuation(Actuation.None);
        }
    }

    private Actuation HandleTrigger(float value, Actuation actuation)
    {
        var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
        handAnimationController?.SetFloat("Trigger", value);

        if (value >= triggerThreshold)
        {
            if (!actuation.HasFlag(Actuation.Trigger))
            {
                actuation |= Actuation.Trigger;
            }
        }
        else
        {
            actuation &= ~Actuation.Trigger;
        }

        return actuation;
    }

    private Actuation HandleGrip(float value, Actuation actuation)
    {
        var handAnimationController = xrController.model.GetComponent<HandAnimationController>() as HandAnimationController;
        handAnimationController?.SetFloat("Grip", value);

        if (value >= gripThreshold)
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
            }
        }
        else
        {
            actuation &= ~Actuation.Grip;
        }

        return actuation;
    }

    private Actuation HandleAXButton(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Button_AX))
            {
                actuation |= Actuation.Button_AX;
            }
        }
        else
        {
            actuation &= ~Actuation.Button_AX;
        }

        return actuation;
    }

    private Actuation HandleAXTouch(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Touch_AX))
            {
                actuation |= Actuation.Touch_AX;
            }
        }
        else
        {
            actuation &= ~Actuation.Touch_AX;
        }

        return actuation;
    }

    private Actuation HandleBYButton(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Button_BY))
            {
                actuation |= Actuation.Button_BY;
            }
        }
        else
        {
            actuation &= ~Actuation.Button_BY;
        }

        return actuation;
    }

        private Actuation HandleBYTouch(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Touch_BY))
            {
                actuation |= Actuation.Touch_BY;
            }
        }
        else
        {
            actuation &= ~Actuation.Touch_BY;
        }

        return actuation;
    }

    private Actuation Handle2DAxis(Vector2 value, Actuation actuation)
    {
        if (value.x <= -thumbstickThreshold.x)
        {
            if (!actuation.HasFlag(Actuation.Thumbstick_Left))
            {
                actuation |= Actuation.Thumbstick_Left;
            }
        }
        else
        {
            actuation &= ~Actuation.Thumbstick_Left;
        }
        
        if (value.x > thumbstickThreshold.x)
        {
            if (!actuation.HasFlag(Actuation.Thumbstick_Right))
            {
                actuation |= Actuation.Thumbstick_Right;
            }
        }
        else
        {
            actuation &= ~Actuation.Thumbstick_Right;
        }

        if (value.y > thumbstickThreshold.y)
        {
            if (!actuation.HasFlag(Actuation.Thumbstick_Up))
            {
                actuation |= Actuation.Thumbstick_Up;
            }
        }
        else
        {
            actuation &= ~Actuation.Thumbstick_Up;
        }
        
        if (value.y <= -thumbstickThreshold.y)
        {
            if (!actuation.HasFlag(Actuation.Thumbstick_Down))
            {
                actuation |= Actuation.Thumbstick_Down;
            }
        }
        else
        {
            actuation &= ~Actuation.Thumbstick_Down;
        }

        return actuation;
    }

    private Actuation Handle2DAxisClick(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Thumbstick_Click))
            {
                actuation |= Actuation.Thumbstick_Click;
            }
        }
        else
        {
            actuation &= ~Actuation.Thumbstick_Click;
        }

        return actuation;
    }

    private Actuation HandleMenuButton(bool value, Actuation actuation)
    {
        if (value)
        {
            if (!actuation.HasFlag(Actuation.Menu_Oculus))
            {
                actuation |= Actuation.Menu_Oculus;
            }
        }
        else
        {
            actuation &= ~Actuation.Menu_Oculus;
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

        lastActuation = actuation;

        Log($"{this.gameObject.name} {className}.Update:Interactable : {interactable != null}");

        var gameObject = interactable?.GetGameObject();

        float triggerValue, gripValue;
        bool buttonAXValue, touchAXValue, buttonBYValue, touchBYValue, thumbstickClickValue, menuButtonValue;
        Vector2 thumbstickValue;

        if (controller.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
        {
            actuation = HandleTrigger(triggerValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.grip, out gripValue))
        {
            actuation = HandleGrip(gripValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out buttonAXValue))
        {
            actuation = HandleAXButton(buttonAXValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primaryTouch, out touchAXValue))
        {
            actuation = HandleAXTouch(touchAXValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonBYValue))
        {
            actuation = HandleBYButton(buttonBYValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.secondaryTouch, out touchBYValue))
        {
            actuation = HandleBYTouch(touchBYValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickValue))
        {
            actuation = Handle2DAxis(thumbstickValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out thumbstickClickValue))
        {
            actuation = Handle2DAxisClick(thumbstickClickValue, actuation);
        }

        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonValue))
        {
            actuation = HandleMenuButton(menuButtonValue, actuation);
        }

        if (actuation != lastActuation)
        {
            gameObject?.GetComponent<IActuation>()?.OnActuation(actuation, characteristics);

            // if (gameObject?.TryGetComponent<IActuation>(out IActuation callback))
            // {
            //     callback.OnActuation(actuation, characteristics);
            // }

            bool pinching = (ActuationContains(Actuation.Trigger));
            bool gripping = (ActuationContains(Actuation.Grip));
            IsPinchingOrGripping(pinching | gripping);

            if (ActuationEventReceived != null)
            {
                ActuationEventReceived.Invoke(actuation, characteristics);
            }
        }
        
        if (RawDataEventReceived != null)
        {
            var rawData = new RawData
            {
                triggerValue = triggerValue,
                gripValue = gripValue,
                buttonAXValue = buttonAXValue,
                buttonBYValue = buttonBYValue,
                thumbstickClickValue = thumbstickClickValue,
                menuButtonValue = menuButtonValue,
                thumbstickValue = thumbstickValue
            };

            gameObject?.GetComponent<IRawData>()?.OnRawData(rawData);
            
            // if (gameObject?.TryGetComponent<IRawData>(out IRawData callback))
            // {
            //     callback.OnRawData(rawData);
            // }

            RawDataEventReceived.Invoke(rawData, characteristics);
        }
        
        UpdateState();
    }

    protected virtual void IsPinchingOrGripping(bool pinchingOrGripping) { }

    private bool ActuationContains(Actuation thisActuation)
    {
        return (actuation & thisActuation) == thisActuation;
    }

    private void UpdateState()
    {
        var state = Enum.HandEnums.State.None;

        if (isHovering)
        {
            state |= Enum.HandEnums.State.Hover;
        }
        else
        {
            state &= ~Enum.HandEnums.State.Hover;
        }

        if (isHolding)
        {
            state |= Enum.HandEnums.State.Grip;
        }
        else
        {
            state &= ~Enum.HandEnums.State.Grip;
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
        NotifyOpposingController(Enum.ControllerEnums.State.Hovering, isHovering, interactable);
    }

    public void SetHolding(IInteractable interactable, bool isHolding)
    {
        Log($"{Time.time} {gameObject.name} {className}.SetHolding:Game Object : {interactable.GetGameObject().name} Is Holding : {isHolding}");
 
        this.isHolding = isHolding;
        this.interactable = (isHolding) ? interactable : null;
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