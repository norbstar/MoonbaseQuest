using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.GunInteractableEnums;
using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("References")]
    [SerializeField] GameObject spawnPoint;

    [Header("Prefabs")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject laserFXPrefab;

    [Header("UI")]
    [SerializeField] GunHUDCanvasManager hudCanvasManager;
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;

    [Header("Docking")]
    [SerializeField] bool enableAutoDock = true;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Laser")]
    [SerializeField] bool spawnLaser;

    [Header("Over Load")]
    [SerializeField] float overLoadThreshold;

    [Header("Sockets")]
    [SerializeField] SocketCompatibilityLayerManager socketCompatibilityLayerManager;
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
    private Mode mode;
    private Intent intent;
    private Enum.GunInteractableEnums.State state;
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
    }

    void OnEnable()
    {
        socketCompatibilityLayerManager.EventReceived += OnSocketCompatiblityLayerEvent;
        socketInteractorManager.EventReceived += OnSocketEvent;
    }

    void OnDisable()
    {
        socketCompatibilityLayerManager.EventReceived -= OnSocketCompatiblityLayerEvent;
        socketInteractorManager.EventReceived -= OnSocketEvent;
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

                if (TryGet.TryIdentifyController(interactor, out HandController controller))
                {
                    var renderer = hitPrefabInstance.GetComponent<Renderer>() as Renderer;
                    var device = controller.InputDevice;

                    if ((int) device.characteristics == (int) HandController.LeftHand)
                    {
                        renderer.material.color = Color.red;
                    }
                    else if ((int) device.characteristics == (int) HandController.RightHand)
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
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered");
        Log($"{Time.time} {gameObject.name} {className} 3");

        gameObject.transform.parent = objects;

        if (controller != null)
        {
            Log($"{Time.time} {gameObject.name} {className} 4");
            var device = controller.InputDevice;

            if ((int) device.characteristics == (int) HandController.LeftHand)
            {
                Log($"{Time.time} {gameObject.name} {className} 5");
                hudCanvasManager.transform.localPosition = new Vector3(-Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                Log($"{Time.time} {gameObject.name} {className} 6");
                hudCanvasManager.transform.localPosition = new Vector3(Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }

            if (hipDocksManager.TryIsDocked(gameObject, out HipDocksManager.DockID dockID))
            {
                Log($"{Time.time} {gameObject.name} {className} 7");
                hipDocksManager.UndockWeapon(gameObject);
            }

            hudCanvasManager.gameObject.SetActive(true);

            // if (cameraManager.TryGetOpposingHandController(controller, out HandController opposingController))
            // {
            //     if (opposingController.IsHolding)
            //     {
            //         var interactable = opposingController.Interactable;
                    
            //         if (interactable.GetGameObject().CompareTag("Flashlight"))
            //         {
            //             HandleHoldingState(true, interactable);
            //         }
            //     }
            // }

            Log($"{Time.time} {gameObject.name} {className} 8");
        }

        Log($"{Time.time} {gameObject.name} {className} 9");

        // socketInteractorManager.EnableCollider(true);
    }

    public void OnActivated(ActivateEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActivated");

        if (mode == Enum.GunInteractableEnums.Mode.Manual)
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
        if (state == Enum.GunInteractableEnums.State.Inactive)
        {
            AudioSource.PlayClipAtPoint(overloadedClip, transform.position, 1.0f);
            return;
        }

        if (hudCanvasManager.AmmoCount == 0) return;

        animator.SetTrigger("Fire");
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);

        if (TryGet.TryIdentifyController(interactor, out HandController controller))
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
            interactableEvent.OnActivate(interactable, transform, lastObjectHitPoint);
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
            state = Enum.GunInteractableEnums.State.Inactive;
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
            state = Enum.GunInteractableEnums.State.Active;
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
        Log($"{Time.time} {gameObject.name} {className} OnDeactivated");

        if (fireRepeatCoroutine != null)
        {
            StopCoroutine(fireRepeatCoroutine);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited");

        hudCanvasManager.gameObject.SetActive(false);
        socketInteractorManager.EnablePreview(false);
        // socketInteractorManager.EnableCollider(false);

        if (enableAutoDock && (controller != null))
        {
            DockWeapon(controller);
        }
    }

    private void DockWeapon(HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} DockWeapon");

        var device = controller.InputDevice;

        if ((int) device.characteristics == (int) HandController.LeftHand)
        {
            hipDocksManager.DockWeapon(gameObject, HipDocksManager.DockID.Left, Quaternion.Euler(90f, 0f, 0f));
        }
        else if ((int) device.characteristics == (int) HandController.RightHand)
        {
            hipDocksManager.DockWeapon(gameObject, HipDocksManager.DockID.Right, Quaternion.Euler(90f, 0f, 0f));
        }
    }

    public void OnActuation(Actuation actuation, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");

        if (!IsHeld) return;

        if (actuation.HasFlag(Actuation.Button_AX))
        {
            AlternateMode();
        }
        else if (actuation.HasFlag(Actuation.Button_BY))
        {
            AlternateIntent();
        }
    }

    private void SetMode(Enum.GunInteractableEnums.Mode mode)
    {
        Log($"{Time.time} {gameObject.name} {className} SetMode: {mode}");

        switch (mode)
        {
            case Enum.GunInteractableEnums.Mode.Manual:
                if (this.mode != Enum.GunInteractableEnums.Mode.Manual)
                {
                    AudioSource.PlayClipAtPoint(manualClip, transform.position, 1.0f);
                }
                break;

            case Enum.GunInteractableEnums.Mode.Auto:
                if (this.mode != Enum.GunInteractableEnums.Mode.Auto)
                {
                    AudioSource.PlayClipAtPoint(autoClip, transform.position, 1.0f);
                }
                break;
        }
        
        hudCanvasManager.SetMode(mode);
        this.mode = mode;
    }

    private void AlternateMode()
    {
        Log($"{Time.time} {gameObject.name} {className} AlternateMode");

        var altMode = (mode == Enum.GunInteractableEnums.Mode.Manual) ? Enum.GunInteractableEnums.Mode.Auto : Enum.GunInteractableEnums.Mode.Manual;
        SetMode(altMode);
    }

    private void SetIntent(Enum.GunInteractableEnums.Intent intent)
    {
        Log($"{Time.time} {gameObject.name} {className} Intent: {intent}");
        
        if (!socketInteractorManager.IsOccupied) return;

        var dockedObject = socketInteractorManager.Data.gameObject;

        if (dockedObject.TryGetComponent<FlashlightInteractableManager>(out var manager))
        {
            switch (intent)
            {
                case Enum.GunInteractableEnums.Intent.Engaged:
                    if (this.intent != Enum.GunInteractableEnums.Intent.Engaged)
                    {
                        manager.State = FlashlightInteractableManager.ActiveState.On;
                        AudioSource.PlayClipAtPoint(engagedClip, transform.position, 1.0f);
                    }
                    break;

                case Enum.GunInteractableEnums.Intent.Disengaged:
                    if (this.intent != Enum.GunInteractableEnums.Intent.Disengaged)
                    {
                        manager.State = FlashlightInteractableManager.ActiveState.Off;
                        AudioSource.PlayClipAtPoint(disengagedClip, transform.position, 1.0f);            
                    }
                    break;
            }

            hudCanvasManager.SetIntent(intent);
            this.intent = intent;
        }
    }

    private void AlternateIntent()
    {
        Log($"{Time.time} {gameObject.name} {className} AlternateIntent");

        if (!socketInteractorManager.IsOccupied) return;
        
        var altIntent = (intent == Enum.GunInteractableEnums.Intent.Engaged) ? Enum.GunInteractableEnums.Intent.Disengaged : Enum.GunInteractableEnums.Intent.Engaged;
        SetIntent(altIntent);
    }

    public void OnSocketCompatiblityLayerEvent(SocketCompatibilityLayerManager manager, SocketCompatibilityLayerManager.EventType eventType, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatiblityLayer:GameObject : {gameObject.name} Type : {eventType}");

        switch (eventType)
        {
            case SocketCompatibilityLayerManager.EventType.OnTriggerEnter:
                OnSocketCompatibilityLayerEntryEvent(gameObject);
                break;
            
            case SocketCompatibilityLayerManager.EventType.OnTriggerExit:
                OnSocketCompatibilityLayerExitEvent(gameObject);
                break;
        }
    }

    public void OnSocketEvent(SocketInteractorManager manager, SocketInteractorManager.EventType eventType, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketEvent:GameObject : {gameObject.name} Type : {eventType}");

        switch (eventType)
        {
            case SocketInteractorManager.EventType.OnHoverEntered:
                OnSocketHoverEntryEvent(gameObject);
                break;

            case SocketInteractorManager.EventType.OnSelectEntered:
                OnSocketSelectEntryEvent(gameObject);
                break;

            case SocketInteractorManager.EventType.OnSelectExited:
                OnSocketSelectExitEvent(gameObject);
                break;

            case SocketInteractorManager.EventType.OnHoverExited:
                OnSocketHoverExitEvent(gameObject);
                break;
        }
    }

    public override void OnOpposingEvent(Enum.ControllerEnums.State state, bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.OnOpposingEvent:State : {state} Is True: {isTrue} GameObject : {obj.GetGameObject().name}");

        if (!IsHeld) return;

        // switch (state)
        // {
        //     case HandController.State.Hovering:
        //         break;

        //     case HandController.State.Holding:
        //         HandleHoldingState(isTrue, obj);
        //         break;
        // }
    }

    private void HandleHoldingState(bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.HandleHoldingState:Is True: {isTrue} GameObject : {obj.GetGameObject().name}");
        
        var gameObject = obj.GetGameObject();

        if ((!socketInteractorManager.IsOccupied) && (gameObject.CompareTag("Flashlight")))
        {
            socketInteractorManager.EnablePreview(isTrue);
        }
    }

    private void OnSocketCompatibilityLayerEntryEvent(GameObject gameObject)
    {
        if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
        {
            gameObject = rootGameObject;
        }
        
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerEntryEvent:GameObject : {gameObject.name}");

        if ((!socketInteractorManager.IsOccupied) && (gameObject.CompareTag("Flashlight")))
        {
            if (TryGet.TryIdentifyController(interactor, out HandController controller))
            {
                if (TryGet.TryGetOpposingController(controller, out HandController opposingController))
                {
                    if ((opposingController.IsHolding) && (GameObject.ReferenceEquals(opposingController.Interactable.GetGameObject(), gameObject)))
                    {
                        socketInteractorManager.EnablePreview(true);

                        if (gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
                        {
                            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Gun Compatible Flashlight" });
                            socketInteractorManager.EnableSocket(true);
                        }
                    }
                }
            }
        }
    }

    private void OnSocketHoverEntryEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketHoverEntryEvent:GameObject : {gameObject.name}");
    }

    private void OnSocketSelectEntryEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:GameObject : {gameObject.name}");

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
                hudCanvasManager.SetIntent(Enum.GunInteractableEnums.Intent.Engaged);
                this.intent = Enum.GunInteractableEnums.Intent.Engaged;
            }
            else
            {
                hudCanvasManager.SetIntent(Enum.GunInteractableEnums.Intent.Disengaged);
                this.intent = Enum.GunInteractableEnums.Intent.Disengaged;
            }
        }

        dockedOccupied = true;
        docked = gameObject;
    }

    private void OnSocketSelectExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketSelectExitEvent:GameObject : {gameObject.name}");

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

        hudCanvasManager.SetIntent(Enum.GunInteractableEnums.Intent.Disengaged);
        this.intent = Enum.GunInteractableEnums.Intent.Disengaged;
        
        dockedOccupied = false;
        docked = null;
    }

    private void OnSocketHoverExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketHoverExitEvent:GameObject : {gameObject.name}");
    }

    private void OnSocketCompatibilityLayerExitEvent(GameObject gameObject)
    {
        if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
        {
            gameObject = rootGameObject;
        }

        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerExitEvent:GameObject : {gameObject.name}");

        if (gameObject.CompareTag("Flashlight"))
        {
            if (TryGet.TryIdentifyController(interactor, out HandController controller))
            {
                if (TryGet.TryGetOpposingController(controller, out HandController opposingController))
                {
                    if ((opposingController.IsHolding) && (GameObject.ReferenceEquals(opposingController.Interactable.GetGameObject(), gameObject)))
                    {
                        socketInteractorManager.EnablePreview(false);

                        if (gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
                        {
                            socketInteractorManager.EnableSocket(false);
                            interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Flashlight" });
                        }
                    }
                }
            }
        }
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