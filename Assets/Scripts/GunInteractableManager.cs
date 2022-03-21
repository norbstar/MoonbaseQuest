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
    [SerializeField] GunHUDCanvasManager hudCanvasManager;
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
    [SerializeField] SocketCompatibilityLayerManager socketCompatibilityLayerManager;
    // [SerializeField] List<SocketCompatibilityLayerManager> socketCompatibilityLayerManagers;

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
    public AudioClip EnagagedClip { get { return engagedClip;} }

    [SerializeField] AudioClip disengagedClip;
    public AudioClip DisengagedClip { get { return disengagedClip;} }

    [SerializeField] AudioClip navigationClip;
    public AudioClip NavigationClip { get { return navigationClip;} }

    public Mode Mode { get { return mode; } set { mode = value; } }
    
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

            EnableDefaultHUD();

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

        var activeIdx = activeHUDManager.ActiveIdx;
        var hudManager = hudManagers[activeIdx];

        // switch (hudManager.Id)
        // {
        //     case Interactables.Gun.HUDManager.Identity.Primary:
        //         break;
            
        //     case Interactables.Gun.HUDManager.Identity.Flashlight:
        //         break;

        //     case Interactables.Gun.HUDManager.Identity.LaserSight:
        //         break;
        // }

        hudManager.OnActuation(actuation);
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

        activeHUDManager.AddHUD((int) Interactables.Gun.HUDCanvasManager.Identity.Flashlight);

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
        
        if (activeHUDManager.RemoveHUD((int) Interactables.Gun.HUDCanvasManager.Identity.Flashlight))
        {
            EnableDefaultHUD();
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