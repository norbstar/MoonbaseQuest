using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.GunInteractableEnums;
using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    private enum Hub
    {
        Primary,
        Flashlight,
        LaserSight
    }

    private class ActiveHUBManager
    {
        private List<int> activeHUBs;
        private int activeIdx;

        public ActiveHUBManager()
        {
            activeHUBs = new List<int>();
        }

        public List<int> ActiveHUBs { get { return activeHUBs; } }

        public bool AddHUB(int idx)
        {
            Debug.Log($"{Time.time} {className} AddHUB 1 Index : {idx}");

            var matches = activeHUBs.Where(i => i == idx);
            
            if (matches.Count() == 0)
            {
                Debug.Log($"{Time.time} {className} AddHUB 2");

                activeHUBs.Add(idx);
                return true;
            }

            Debug.Log($"{Time.time} {className} AddHUB 3");

            return false;
        }

        public bool RemoveHUB(int idx)
        {
            var matches = activeHUBs.Where(i => i == idx);
            
            if (matches.Count() == 1)
            {
                activeHUBs.Remove(idx);
                return true;
            }

            return false;
        }

        public int ActiveIdx { get { return activeIdx; } set { activeIdx = value; } }

        public bool TryGetPreviousIndex(out int idx)
        {
            int tempIdx = activeIdx;
            bool resolvedIdx = false;

            do
            {
                --tempIdx;

                if (tempIdx < 0)
                {
                    tempIdx = activeHUBs.Count - 1;
                }

                if (tempIdx != activeIdx)
                {
                    resolvedIdx = true;
                }
            } while (!resolvedIdx && tempIdx != activeIdx);

            idx = tempIdx;
            return resolvedIdx;
        }

        public bool TryGetNextIndex(out int idx)
        {
            int tempIdx = activeIdx;
            bool resolvedIdx = false;

            do
            {
                ++tempIdx;

                if (tempIdx > activeHUBs.Count - 1)
                {
                    tempIdx = 0;
                }

                if (tempIdx != activeIdx)
                {
                    resolvedIdx = true;
                }
            } while (!resolvedIdx && tempIdx != activeIdx);

            idx = tempIdx;
            return resolvedIdx;
        }
    }

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("References")]
    [SerializeField] GameObject spawnPoint;

    [Header("Prefabs")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject laserFXPrefab;

    [Header("UI")]
    [SerializeField] GunHUDCanvasManager hudCanvasManager;
    [SerializeField] List<Interactables.Gun.HUDCanvasManager> hudCanvasManagers;
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
    // [SerializeField] List<SocketCompatibilityLayerManager> socketCompatibilityLayerManagers;

    [Header("Audio")]
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip manualClip;
    [SerializeField] AudioClip autoClip;
    [SerializeField] AudioClip overloadedClip;
    [SerializeField] AudioClip engagedClip;
    [SerializeField] AudioClip disengagedClip;
    [SerializeField] AudioClip navigationClip;

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
    private bool socketOccupied;
    private GameObject docked;
    private ActiveHUBManager activeHUBManager;

    protected override void Awake()
    {
        base.Awake();
        
        camera = Camera.main;
        ResolveDependencies();
        CacheGunState();

        mixedLayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Asteroid Layer");
        overheatCanvasManager.SetMaxValue(overLoadThreshold);
        heatValues = curveCreator.Values;

        activeHUBManager = new ActiveHUBManager();
        activeHUBManager.AddHUB((int) Hub.Primary);

        StartCoroutine(ManageHeatCoroutine());
    }

    private void ResolveDependencies()
    {
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
        cameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;
        hipDocksManager = cameraManager.HipDocksManager;
    }

    private bool TryGetSocketInteractorManager(SocketCompatibilityLayerManager manager, out SocketInteractorManager socketInteractorManager)
    {
        socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
        return (socketInteractorManager != null);
    }

    void OnEnable()
    {
        socketCompatibilityLayerManager.EventReceived += OnSocketCompatiblityLayerEvent;

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            socketInteractorManager.EventReceived += OnSocketEvent;
        }

        // foreach (SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        // {
        //     manager.EventReceived += OnSocketCompatiblityLayerEvent;

        //     var socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
            
        //     if (socketInteractorManager != null)
        //     {
        //         socketInteractorManager.EventReceived += OnSocketEvent;
        //     }
        // }
    }

    void OnDisable()
    {
        socketCompatibilityLayerManager.EventReceived -= OnSocketCompatiblityLayerEvent;

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            socketInteractorManager.EventReceived -= OnSocketEvent;
        }

        // foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        // {
        //     manager.EventReceived -= OnSocketCompatiblityLayerEvent;

        //     var socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
            
        //     if (socketInteractorManager != null)
        //     {
        //         socketInteractorManager.EventReceived -= OnSocketEvent;
        //     }
        // }
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

        gameObject.transform.parent = objects;

        if (controller != null)
        {
            var device = controller.InputDevice;

            if ((int) device.characteristics == (int) HandController.LeftHand)
            {
                hudCanvasManager.transform.localPosition = new Vector3(-Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                hudCanvasManager.transform.localPosition = new Vector3(Mathf.Abs(hudCanvasManager.transform.localPosition.x), 0.06f, 0f);
            }

            ShowDefaultHUBCanvas();

            if (hipDocksManager.TryIsDocked(gameObject, out HipDocksManager.DockID dockID))
            {
                hipDocksManager.UndockWeapon(gameObject);
            }

            hudCanvasManager.gameObject.SetActive(true);
        }
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

        // foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        // {
        //     var socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
        //     socketInteractorManager?.EnablePreview(false);
        // }

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            socketInteractorManager?.EnablePreview(false);
        }

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

        if (actuation.HasFlag(Actuation.Thumbstick_Left) || actuation.HasFlag(Actuation.Thumbstick_Right))
        {
            HandleUINagivation(actuation);
        }

        if (actuation.HasFlag(Actuation.Button_AX))
        {
            AlternateMode();
        }
        
        if (actuation.HasFlag(Actuation.Button_BY))
        {
            AlternateIntent();
        }
    }

    private void HandleUINagivation(Actuation actuation)
    {
        bool success = false;
        int idx = 0;

        if (actuation.HasFlag(Actuation.Thumbstick_Left))
        {
            if (activeHUBManager.TryGetPreviousIndex(out idx))
            {
                success = true;
            }
        }
        else if (actuation.HasFlag(Actuation.Thumbstick_Right))
        {
            if (activeHUBManager.TryGetNextIndex(out idx))
            {
                success = true;
            }
        }

        if (success)
        {
            AudioSource.PlayClipAtPoint(navigationClip, transform.position, 1.0f);
            ShowHUBCanvas(idx);
        }
    }

    private void ShowDefaultHUBCanvas()
    {
        ShowHUBCanvas((int) Hub.Primary);
    }

    private void ShowHUBCanvas(int idx)
    {
        Interactables.Gun.HUDCanvasManager hubCanvasManager;

        hubCanvasManager = hudCanvasManagers[activeHUBManager.ActiveIdx];
        hubCanvasManager.gameObject.SetActive(false);

        hubCanvasManager = hudCanvasManagers[idx];
        hubCanvasManager.gameObject.SetActive(true);

        activeHUBManager.ActiveIdx = idx;
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
        
        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
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
    }

    private void AlternateIntent()
    {
        Log($"{Time.time} {gameObject.name} {className} AlternateIntent");

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            if (!socketInteractorManager.IsOccupied) return;
        }
        
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

        switch (state)
        {
            case Enum.ControllerEnums.State.Hovering:
                break;

            case Enum.ControllerEnums.State.Holding:
                HandleHoldingState(isTrue, obj);
                break;
        }
    }

    private void HandleHoldingState(bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.HandleHoldingState:Is True: {isTrue} GameObject : {obj.GetGameObject().name}");
        
        var gameObject = obj.GetGameObject();

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            if ((!socketInteractorManager.IsOccupied) && (gameObject.CompareTag("Compact Flashlight")))
            {
                socketInteractorManager.EnablePreview(isTrue);
            }
        }
    }

    private void OnSocketCompatibilityLayerEntryEvent(GameObject gameObject)
    {
        if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
        {
            gameObject = rootGameObject;
        }
        
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerEntryEvent:GameObject : {gameObject.name}");

        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        {
            if ((!socketInteractorManager.IsOccupied) && (gameObject.CompareTag("Compact Flashlight")))
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

        if (activeHUBManager.AddHUB((int) Hub.Flashlight))
        {
            Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:Flashlight hub was added");
        }
        else
        {
            Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:Flashlight hub was NOT added");
        }

        socketOccupied = true;
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
        
        if (activeHUBManager.RemoveHUB((int) Hub.Flashlight))
        {
            Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:Flashlight hub was removed");
            ShowDefaultHUBCanvas();
        }
        else
        {
            Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:Flashlight hub was NOT removed");
        }

        socketOccupied = false;
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

        if (gameObject.CompareTag("Compact Flashlight"))
        {
            if (TryGet.TryIdentifyController(interactor, out HandController controller))
            {
                if (TryGet.TryGetOpposingController(controller, out HandController opposingController))
                {
                    if ((opposingController.IsHolding) && (GameObject.ReferenceEquals(opposingController.Interactable.GetGameObject(), gameObject)))
                    {
                        if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
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