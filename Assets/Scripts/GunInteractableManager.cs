using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static GunInteractableEnums;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusableInteractableManager, IGesture
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    public static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("References")]
    [SerializeField] GameObject spawnPoint;

    [Header("Prefabs")]
    [SerializeField] GameObject laserPrefab, laserFXPrefab;

    [Header("UI")]
    [SerializeField] GunHUDCanvasManager hudCanvasManager;
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;
    [SerializeField] float speed = 5f;

    [Header("Optional Settings")]
    [SerializeField] bool enableAutoDock = true;
    [SerializeField] bool enableGravityOnGrab = true;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Laser")]
    [SerializeField] bool spawnLaser;

    [Header("Over Load")]
    [SerializeField] float overLoadThreshold;

    [Header("Sockets")]
    [SerializeField] SocketInteractorManager socketInteractorManager;

    [Header("Audio")]
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip manualClip;
    [SerializeField] AudioClip autoClip;
    [SerializeField] AudioClip overloadedClip;
    [SerializeField] AudioClip engagedClip;
    [SerializeField] AudioClip disengagedClip;

    private new Camera camera;
    private CurveCreator curveCreator;
    private MainCameraManager cameraManager;
    private HipDocksManager hipDocksManager;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private GameObject lastObjectHit;
    private Vector3 lastObjectHitPoint;
    private IFocus lastFocus;
    private GameObject hitPrefabInstance;
    private int mixedLayerMask;
    private GunInteractableEnums.Mode mode;
    private GunInteractableEnums.Intent intent;
    private GunInteractableEnums.State state;
    private Coroutine fireRepeatCoroutine;
    private float heat;
    private IList<float> heatValues;
    private int heatIndex;
    private bool dockedOccupied;
    private GameObject docked;

    protected override void Awake()
    {
        base.Awake();
        
        camera = Camera.main;
        ResolveDependencies();
        CacheGunState();

        socketInteractorManager.EventReceived += OnSocketEvent;

        mixedLayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Asteroid Layer");
        overheatCanvasManager.SetMaxValue(overLoadThreshold);
        heatValues = curveCreator.Values;

        StartCoroutine(ManageHeatCoroutine());
    }

    private void ResolveDependencies()
    {
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
        cameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;
        hipDocksManager = cameraManager.HipDocksManager;
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    void FixedUpdate()
    {
        var ray = new Ray(spawnPoint.transform.position, spawnPoint.transform.forward);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, mixedLayerMask))
        {
            var objectHit = hit.transform.gameObject;
            var point = hit.point;

            if (showHits)
            {
                if (hitPrefabInstance == null)
                {
                    hitPrefabInstance = Instantiate(hitPrefab, point, Quaternion.identity);
                }
                else
                {
                    hitPrefabInstance.transform.position = point;
                    hitPrefabInstance.SetActive(true);
                }

                if (TryGetController<HandController>(interactor, out HandController controller))
                {
                    var renderer = hitPrefabInstance.GetComponent<Renderer>() as Renderer;
                    var device = controller.GetInputDevice();

                    if (((int) device.characteristics) == ((int) LeftHand))
                    {
                        renderer.material.color = Color.red;
                    }
                    else if (((int) device.characteristics) == ((int) RightHand))
                    {
                        renderer.material.color = Color.blue;
                    }
                }
            }

            if (!GameObject.ReferenceEquals(objectHit, lastObjectHit))
            {
                if (lastFocus != null)
                {
                    lastFocus.LostFocus(gameObject);
                    lastFocus = null;
                }

                if (objectHit.TryGetComponent<IFocus>(out IFocus focus))
                {
                    focus.GainedFocus(gameObject);
                    lastFocus = focus;
                }            

                lastObjectHit = objectHit;
            }

            lastObjectHitPoint = point;
        }
        else
        {
            if (lastFocus != null)
            {
                lastFocus.LostFocus(gameObject);
                lastFocus = null;
            }

            hitPrefabInstance?.SetActive(false);
            lastObjectHit = null;
            lastObjectHitPoint = default(Vector3);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args, HandController controller)
    {
        gameObject.transform.parent = objects;

        if (controller != null)
        {
            var device = controller.GetInputDevice();

            if (((int) device.characteristics) == ((int) LeftHand))
            {
                hudCanvasManager.transform.localPosition = new Vector3(-Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }
            else if (((int) device.characteristics) == ((int) RightHand))
            {
                hudCanvasManager.transform.localPosition = new Vector3(Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }

            if (hipDocksManager.TryIsDocked(gameObject, out HipDocksManager.DockID dockID))
            {
                hipDocksManager.UndockWeapon(gameObject);
            }

            hudCanvasManager.gameObject.SetActive(true);
        }

        if (enableGravityOnGrab)
        {
            enableGravityOnGrab = false;
            cache.isKinematic = false;
            cache.useGravity = true;
        }
    }

    public void OnActivated(ActivateEventArgs args)
    {
        if (mode == GunInteractableEnums.Mode.Manual)
        {
            FireOnce();
        }
        else
        {
            fireRepeatCoroutine = StartCoroutine(FireRepeat());
        }
    }

    private void FireOnce()
    {
        Fire();
    }

    private IEnumerator FireRepeat()
    {
        while (hudCanvasManager.AmmoCount > 0)
        {
            Fire();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Fire()
    {
        if (state == State.Inactive)
        {
            AudioSource.PlayClipAtPoint(overloadedClip, transform.position, 1.0f);
            return;
        }

        if (hudCanvasManager.AmmoCount == 0) return;

        animator.SetTrigger("Fire");
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);

        if (TryGetController<HandController>(interactor, out HandController controller))
        {
            controller.SetImpulse();
        }

        if (spawnLaser)
        {
            GameObject.Instantiate(laserPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        else
        {
            var laserInstance = GameObject.Instantiate(laserFXPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            laserInstance.transform.parent = gameObject.transform;
        }

        if ((lastObjectHit != null) && (lastObjectHit.TryGetComponent<IInteractableEvent>(out IInteractableEvent interactableEvent)))
        {
            interactableEvent.OnActivate(interactable, lastObjectHitPoint);
        }

        hudCanvasManager.DecrementAmmoCount();
        IncreaseHeat();
    }

    private void IncreaseHeat()
    {
        var currentValue = heatValues[heatIndex];

        if (heatIndex + 1 < heatValues.Count)
        {
            heatIndex += 1;
            heat = heatValues[heatIndex];
            overheatCanvasManager.SetValue(heat);
        }
        else
        {
            state = State.Inactive;
        }
    }

    private void DecreaseHeat()
    {
        var currentValue = heatValues[heatIndex];

        if (heatIndex - 1 >= 0)
        {
            heatIndex -= 1;
            heat = heatValues[heatIndex];
            overheatCanvasManager.SetValue(heat);
        }

        if (heatIndex < heatValues.Count)
        {
            state = State.Active;
        }
    }

    private IEnumerator ManageHeatCoroutine()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(1f);
            DecreaseHeat();
        }
    }

    public void OnDeactivated(DeactivateEventArgs args)
    {
        if (fireRepeatCoroutine != null)
        {
            StopCoroutine(fireRepeatCoroutine);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        hudCanvasManager.gameObject.SetActive(false);

        if (enableAutoDock && (controller != null))
        {
            DockWeapon(controller);
        }
    }

    private void DockWeapon(HandController controller)
    {
        var device = controller.GetInputDevice();

        if (((int) device.characteristics) == ((int) LeftHand))
        {
            hipDocksManager.DockWeapon(gameObject, HipDocksManager.DockID.Left, Quaternion.Euler(90f, 0f, 0f));
        }
        else if (((int) device.characteristics) == ((int) RightHand))
        {
            hipDocksManager.DockWeapon(gameObject, HipDocksManager.DockID.Right, Quaternion.Euler(90f, 0f, 0f));
        }
    }

    public void OnGesture(HandController.Gesture gesture, object value = null)
    {
        switch (gesture)
        {
            case HandController.Gesture.ThumbStick_Left:
                SetMode(Mode.Manual);
                break;
            
            case HandController.Gesture.ThumbStick_Right:
                SetMode(Mode.Auto);
                break;

            case HandController.Gesture.ThumbStick_Up:
                SetIntent(Intent.Engaged);
                break;

            case HandController.Gesture.ThumbStick_Down:
                SetIntent(Intent.Disengaged);
                break;
        }
    }

    private void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Manual:
                if (this.mode != Mode.Manual)
                {
                    AudioSource.PlayClipAtPoint(manualClip, transform.position, 1.0f);
                    hudCanvasManager.SetMode(mode);
                    this.mode = mode;
                }
                break;

            case Mode.Auto:
                if (this.mode != Mode.Auto)
                {
                    AudioSource.PlayClipAtPoint(autoClip, transform.position, 1.0f);
                    hudCanvasManager.SetMode(mode);
                    this.mode = mode;
                }
                break;
        }
    }

    private void SetIntent(Intent intent)
    {
        if (!socketInteractorManager.Data.occupied) return;

        Log($"{Time.time} {gameObject.name} {className} Intent: {intent}");

        var dockedObject = socketInteractorManager.Data.gameObject;

        if (dockedObject.TryGetComponent<FlashlightInteractableManager>(out var manager))
        {
            switch (intent)
            {
                case Intent.Engaged:
                    if (this.intent != Intent.Engaged)
                    {
                        manager.State = FlashlightInteractableManager.ActiveState.On;
                        AudioSource.PlayClipAtPoint(engagedClip, transform.position, 1.0f);
                        hudCanvasManager.SetIntent(intent);
                        this.intent = intent;
                    }
                    break;

                case Intent.Disengaged:
                    if (this.intent != Intent.Disengaged)
                    {
                        manager.State = FlashlightInteractableManager.ActiveState.Off;
                        AudioSource.PlayClipAtPoint(disengagedClip, transform.position, 1.0f);
                        hudCanvasManager.SetIntent(intent);
                        this.intent = intent;
                    }
                    break;
            }
        }
    }

    public void OnSocketEvent(SocketInteractorManager manager, SocketInteractorManager.EventType eventType, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketEvent:GameObject : {gameObject.name} Type : {eventType}");

        switch (eventType)
        {
            case SocketInteractorManager.EventType.OnDocked:
                OnDocked(gameObject);
                break;

            case SocketInteractorManager.EventType.OnUndocked:
                OnUndocked(gameObject);
                break;
        }
    }

    public override void OnOpposingEvent(HandController.State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.OnOpposingEvent:State : {state} GameObject : {obj.GetGameObject().name}");

        if (!IsHeld) return;

        var gameObject = obj.GetGameObject();

        switch (state)
        {
            case HandController.State.Holding:
                if ((!socketInteractorManager.Data.occupied) && (gameObject.CompareTag("Flashlight")))
                {
                    socketInteractorManager.Reveal(isTrue);
                    
                    if (gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
                    {
                        // var testA = InteractionLayerMask.GetMask(new string[] { "Default", "Flashlight" });
                        // Log($"{Time.time} {gameObject.name} {className}.TestA : {testA}");
                        // var testB = InteractionLayerMask.GetMask(new string[] { "Default", "Gun Attached Flashlight" });
                        // Log($"{Time.time} {gameObject.name} {className}.TestB : {testB}");
                        // int layer1 = InteractionLayerMask.NameToLayer("Default");
                        // Log($"{Time.time} {gameObject.name} {className}.Default Layer : {layer1}");
                        // int layer2 = InteractionLayerMask.NameToLayer("Gun Attached Flashlight");
                        // Log($"{Time.time} {gameObject.name} {className}.Gun Attached Flashlight Layer : {layer2}");

                        // Log($"{Time.time} {gameObject.name} {className}.Pre Value : {interactable.interactionLayers.value}");
                        
                        if (isTrue)
                        {                    
                            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Gun Compatible Flashlight" });
                            Log($"{Time.time} {gameObject.name} {className}.InteractionLayers:{interactable.interactionLayers.value}");
                        }
                        else
                        {
                            StartCoroutine(DefaultInteractionLayers(interactable));
                        }

                        // Log($"{Time.time} {gameObject.name} {className}.Post Value : {interactable.interactionLayers.value}");
                    }
                }
                break;
        }
    }

    private IEnumerator DefaultInteractionLayers(XRGrabInteractable interactable)
    {
        Log($"{Time.time} {gameObject.name} {className}.DefaultInteractionLayers");

        yield return new WaitForSeconds(1);

        if (!socketInteractorManager.Data.occupied || !Object.ReferenceEquals(interactable.gameObject, socketInteractorManager.Data.gameObject))
        {
            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Flashlight" });
        }
    }

    private void OnDocked(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnDocked:GameObject : {gameObject.name}");

        if (gameObject.TryGetComponent<FlashlightInteractableManager>(out var flashlightManager))
        {
            /*
            Optional implementation that notifies the child docked flashlight of
            the stage change. This is however not required as the socket interactor manager
            notifies the flashlight instance directly via OnDockStatusChange.
            */
#if false
            flashlightManager.OnDockStatusChange(true);
#endif
            if (flashlightManager.State == FlashlightInteractableManager.ActiveState.On)
            {
                hudCanvasManager.SetIntent(Intent.Engaged);
                this.intent = Intent.Engaged;
            }
            else
            {
                hudCanvasManager.SetIntent(Intent.Disengaged);
                this.intent = Intent.Disengaged;
            }
        }

        dockedOccupied = true;
        docked = gameObject;
    }

    private void OnUndocked(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnUndocked:GameObject : {gameObject.name}");

        /*
        Optional implementation that notifies the child docked flashlight of
        the stage change. This is however not required as the socket interactor manager
        notifies the flashlight instance directly via OnDockStatusChange.
        */
#if false
        if (gameObject.TryGetComponent<FlashlightInteractableManager>(out var flashlightManager))
        {
            flashlightManager.OnDockStatusChange(false);
        }
#endif

        hudCanvasManager.SetIntent(Intent.Disengaged);
        this.intent = Intent.Disengaged;
        
        dockedOccupied = false;
        docked = null;
    }

    public void RestoreCachedGunState()
    {
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
    }

    private void CacheGunState()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    public void RestoreAmmoCount() => hudCanvasManager.RestoreAmmoCount();
}