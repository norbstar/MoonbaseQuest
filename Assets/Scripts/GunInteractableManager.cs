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

    private class ActiveHUDManager
    {
        private List<int> activeHUDs;
        private int activeIdx;

        public ActiveHUDManager()
        {
            activeHUDs = new List<int>();
        }

        public List<int> ActiveHUDs { get { return activeHUDs; } }

        public bool AddHUD(int idx)
        {
            var matches = activeHUDs.Where(i => i == idx);
            
            if (matches.Count() == 0)
            {
                activeHUDs.Add(idx);
                return true;
            }

            return false;
        }

        public bool RemoveHUD(int idx)
        {
            var matches = activeHUDs.Where(i => i == idx);
            
            if (matches.Count() == 1)
            {
                activeHUDs.Remove(idx);
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
                    tempIdx = activeHUDs.Count - 1;
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

                if (tempIdx > activeHUDs.Count - 1)
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
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;
    [SerializeField] List<Interactables.Gun.HUDManager> hudManagers;

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
    // [SerializeField] SocketCompatibilityLayerManager socketCompatibilityLayerManager;
    // public SocketCompatibilityLayerManager SocketCompatibilityLayerManager { get { return socketCompatibilityLayerManager; } }

    [SerializeField] List<SocketCompatibilityLayerManager> socketCompatibilityLayerManagers;
    public List<SocketCompatibilityLayerManager> SocketCompatibilityLayerManagers { get { return socketCompatibilityLayerManagers; } }

    [Header("Audio")]
    [SerializeField] AudioClip hitClip;
    public AudioClip AudioClip { get { return hitClip;} }

    [SerializeField] AudioClip manualClip;
    public AudioClip ManualClip { get { return manualClip;} }

    [SerializeField] AudioClip autoClip;
    public AudioClip AutoClip { get { return autoClip;} }

    [SerializeField] AudioClip overloadedClip;
    public AudioClip OverloadedClip { get { return overloadedClip;} }

    [SerializeField] AudioClip engagedClip;
    public AudioClip EngagedClip { get { return engagedClip;} }

    [SerializeField] AudioClip disengagedClip;
    public AudioClip DisengagedClip { get { return disengagedClip;} }

    [SerializeField] AudioClip navigationClip;
    public AudioClip NavigationClip { get { return navigationClip;} }

    public Mode Mode { get { return mode; } set { mode = value; } }

    private Interactables.Gun.HUDManager HUDManager
    {
        get
        {
            var activeIdx = activeHUDManager.ActiveIdx;
            return hudManagers[activeIdx];
        }
    }

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
    private Enum.GunInteractableEnums.State state;
    private Coroutine fireRepeatCoroutine;
    private float heat;
    private IList<float> heatValues;
    private int heatIndex;
    private ActiveHUDManager activeHUDManager;

    protected override void Awake()
    {
        base.Awake();
        
        camera = Camera.main;
        ResolveDependencies();
        CacheGunState();

        mixedLayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Asteroid Layer");
        overheatCanvasManager.SetMaxValue(overLoadThreshold);
        heatValues = curveCreator.Values;

        activeHUDManager = new ActiveHUDManager();
        activeHUDManager.AddHUD((int) Interactables.Gun.HUDManager.Identity.Primary);

        StartCoroutine(ManageHeatCoroutine());
    }

    private void ResolveDependencies()
    {
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
        cameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;
        hipDocksManager = cameraManager.HipDocksManager;
    }

    public bool TryGetSocketInteractorManager(SocketCompatibilityLayerManager manager, out SocketInteractorManager socketInteractorManager)
    {
        socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
        return (socketInteractorManager != null);
    }

    void OnEnable()
    {
        // socketCompatibilityLayerManager.EventReceived += OnSocketCompatiblityLayerEvent;

        // if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        // {
        //     socketInteractorManager.EventReceived += OnSocketEvent;
        // }

        foreach (SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            manager.EventReceived += OnSocketCompatiblityLayerEvent;

            if (TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
            {
                socketInteractorManager.EventReceived += OnSocketEvent;
            }
        }
    }

    void OnDisable()
    {
        // socketCompatibilityLayerManager.EventReceived -= OnSocketCompatiblityLayerEvent;

        // if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        // {
        //     socketInteractorManager.EventReceived -= OnSocketEvent;
        // }

        foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            manager.EventReceived -= OnSocketCompatiblityLayerEvent;

            if (TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
            {
                socketInteractorManager.EventReceived -= OnSocketEvent;
            }
        }
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
                foreach (Interactables.Gun.HUDManager hudManager in hudManagers)
                {
                    var gameObject = hudManager.GetCanvas().gameObject;
                    gameObject.transform.localPosition = new Vector3(-Mathf.Abs(gameObject.transform.localPosition.x), 0.06f, 0f);
                }
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                foreach (Interactables.Gun.HUDManager hudManager in hudManagers)
                {
                    var gameObject = hudManager.GetCanvas().gameObject;
                    gameObject.transform.localPosition = new Vector3(Mathf.Abs(gameObject.transform.localPosition.x), 0.06f, 0f);
                }
            }

            EnableDefaultHUD();

            if (hipDocksManager.TryIsDocked(gameObject, out HipDocksManager.DockID dockID))
            {
                hipDocksManager.UndockWeapon(gameObject);
            }

            HUDManager.ShowHUD();
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
        while (((Interactables.Gun.PrimaryHUDManager) HUDManager).AmmoCount > 0)
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

        if (((Interactables.Gun.PrimaryHUDManager) HUDManager).AmmoCount == 0) return;

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

        ((Interactables.Gun.PrimaryHUDManager) HUDManager).DecrementAmmoCount();
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

        HUDManager.HideHUD();

        // if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
        // {
        //     socketInteractorManager.EnablePreview(false);
        // }

        foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            if (TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
            {
                socketInteractorManager.EnablePreview(false);
            }
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

        // switch (HUDManager.Id)
        // {
        //     case Interactables.Gun.HUDManager.Identity.Primary:
        //         break;
            
        //     case Interactables.Gun.HUDManager.Identity.Flashlight:
        //         break;

        //     case Interactables.Gun.HUDManager.Identity.LaserSight:
        //         break;
        // }

        HUDManager.OnActuation(actuation);
    }

    private void HandleUINagivation(Actuation actuation)
    {
        bool success = false;
        int idx = 0;

        if (actuation.HasFlag(Actuation.Thumbstick_Left))
        {
            if (activeHUDManager.TryGetPreviousIndex(out idx))
            {
                success = true;
            }
        }
        else if (actuation.HasFlag(Actuation.Thumbstick_Right))
        {
            if (activeHUDManager.TryGetNextIndex(out idx))
            {
                success = true;
            }
        }

        if (success)
        {
            AudioSource.PlayClipAtPoint(navigationClip, transform.position, 1.0f);
            EnableHUD(idx);
        }
    }

    private void EnableDefaultHUD()
    {
        EnableHUD((int) Interactables.Gun.HUDManager.Identity.Primary);
    }

    private void EnableHUD(int idx)
    {
        Interactables.Gun.HUDManager hudManager;

        hudManager = hudManagers[activeHUDManager.ActiveIdx];
        hudManager.HideHUD();

        hudManager = hudManagers[idx];
        hudManager.ShowHUD();

        activeHUDManager.ActiveIdx = idx;
    }

    public void OnSocketCompatiblityLayerEvent(SocketCompatibilityLayerManager manager, SocketCompatibilityLayerManager.EventType eventType, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatiblityLayer:GameObject : {gameObject.name} Type : {eventType}");

        switch (eventType)
        {
            case SocketCompatibilityLayerManager.EventType.OnTriggerEnter:
                OnSocketCompatibilityLayerEntryEvent(manager, gameObject);
                break;
            
            case SocketCompatibilityLayerManager.EventType.OnTriggerExit:
                OnSocketCompatibilityLayerExitEvent(manager, gameObject);
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
            case Enum.ControllerEnums.State.Holding:
                HandleHoldingState(isTrue, obj);
                break;
        }
    }

    public bool TryGetCompatibleLayer(GameObject gameObject, out SocketCompatibilityLayerManager socketCompatibilityLayerManager)
    {
        SocketCompatibilityLayerManager manager = null;

        if (gameObject.CompareTag("Compact Flashlight"))
        {
            manager = socketCompatibilityLayerManagers.Where(s => s.CompareTag("Flashlight Socket")).FirstOrDefault();
        }
        else if (gameObject.CompareTag("Laser Sight"))
        {
            manager = socketCompatibilityLayerManagers.Where(s => s.CompareTag("Laser Sight Socket")).FirstOrDefault();
        }

        socketCompatibilityLayerManager = manager;
        return (manager != null);
    }

    private void HandleHoldingState(bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.HandleHoldingState:Is True: {isTrue} GameObject : {obj.GetGameObject().name}");
        
        var gameObject = obj.GetGameObject();

        if (TryGetCompatibleLayer(gameObject, out SocketCompatibilityLayerManager socketCompatibilityLayerManager))
        {
            if (TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
            {
                if (!socketInteractorManager.IsOccupied)
                {
                    socketInteractorManager.EnablePreview(isTrue);
                }
            }
        }
    }

    private void OnSocketCompatibilityLayerEntryEvent(SocketCompatibilityLayerManager manager, GameObject gameObject)
    {
        if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
        {
            gameObject = rootGameObject;
        }
        
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerEntryEvent:GameObject : {gameObject.name}");

        if (TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
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

            if ((!socketInteractorManager.IsOccupied) && (gameObject.CompareTag("Laser Sight")))
            {
                // TODO
            }
        }
    }

    private bool TryGetHUDManagerById(Interactables.Gun.HUDManager.Identity id, out Interactables.Gun.HUDManager hudManager)
    {
        foreach (var thisHUDManager in hudManagers)
        {
            if (thisHUDManager.Id == id)
            {
                hudManager = thisHUDManager;
                return true;
            }
        }

        hudManager = null;
        return false;
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

            var id = Interactables.Gun.HUDManager.Identity.Flashlight;

            activeHUDManager.AddHUD((int) id);
            
            if (TryGetHUDManagerById(id, out Interactables.Gun.HUDManager hudManager))
            {
                if (flashlightManager.State == FlashlightInteractableManager.ActiveState.On)
                {
                    ((Interactables.Gun.FlashlightHUDManager) hudManager).SetState(Enum.GunInteractableEnums.State.Active);
                }
                else
                {
                    ((Interactables.Gun.FlashlightHUDManager) hudManager).SetState(Enum.GunInteractableEnums.State.Inactive);
                }
            }
        }
    }

    private void OnSocketSelectExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketSelectExitEvent:GameObject : {gameObject.name}");

        if (gameObject.TryGetComponent<FlashlightInteractableManager>(out var flashlightManager))
        {
            /*
            Optional implementation that notifies the child docked flashlight of
            the stage change. This is however not required as the socket interactor manager
            notifies the flashlight instance directly via OnDockStatusChange.
            */
    #if false
        flashlightManager.OnDockStatusChange(false);
    #endif

            var id = Interactables.Gun.HUDManager.Identity.Flashlight;

            if (TryGetHUDManagerById(id, out Interactables.Gun.HUDManager hudManager))
            {
                ((Interactables.Gun.FlashlightHUDManager) hudManager).SetState(Enum.GunInteractableEnums.State.Inactive);
            }
            
            if (activeHUDManager.RemoveHUD((int) id))
            {
                EnableDefaultHUD();
            }
        }

        if (gameObject.TryGetComponent<LaserSightInteractableManager>(out var laserSightManager))
        {
            // TODO
        }
    }

    private void OnSocketHoverExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketHoverExitEvent:GameObject : {gameObject.name}");
    }

    private void OnSocketCompatibilityLayerExitEvent(SocketCompatibilityLayerManager manager, GameObject gameObject)
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
                        if (TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
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

        if (gameObject.CompareTag("Laser Sight"))
        {
            // TODO
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

    public void RestoreAmmoCount() => ((Interactables.Gun.PrimaryHUDManager) HUDManager).RestoreAmmoCount();
}