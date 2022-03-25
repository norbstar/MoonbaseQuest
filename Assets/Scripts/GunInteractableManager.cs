using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.GunInteractableEnums;
using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusableInteractableManager, IActuation, IRawData
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
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;
    [SerializeField] Interactables.Gun.HUDContainerManager hudContainerManager;

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

    [Header("Config")]
    [SerializeField] bool enableQuickHome = true;
    [SerializeField] bool switchHUDOnDock = true;
    [SerializeField] float actuationDelay = 0.5f;

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
    private Enum.GunInteractableEnums.State state;
    private Coroutine fireRepeatCoroutine;
    private float heat;
    private IList<float> heatValues;
    private int heatIndex;
    private Interactables.Gun.HUDsManager hudsManager;
    private Interactables.Gun.HomeHUDManager homeHUD;
    private float lastActuation;

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
        hudsManager = hudContainerManager.HUDsManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        var id = Interactables.Gun.HUDManager.Identity.Home;

        if (hudContainerManager.TryGetHUDManagerById(id, out Interactables.Gun.HUDManager hudManager))
        {
            homeHUD = (Interactables.Gun.HomeHUDManager) hudManager;
        }
    }

    void OnEnable()
    {
        foreach (SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            manager.EventReceived += OnSocketCompatiblityLayerEvent;

            if (TryGet.TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
            {
                socketInteractorManager.EventReceived += OnSocketEvent;
            }
        }
    }

    void OnDisable()
    {
        foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            manager.EventReceived -= OnSocketCompatiblityLayerEvent;

            if (TryGet.TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
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

                if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
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
            var hudContainer = hudContainerManager.gameObject;

            if ((int) device.characteristics == (int) HandController.LeftHand)
            {
                hudContainer.transform.localPosition = new Vector3(-Mathf.Abs(hudContainer.transform.localPosition.x), 0.06f, 0f);
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                hudContainer.transform.localPosition = new Vector3(Mathf.Abs(hudContainer.transform.localPosition.x), 0.06f, 0f);
            }

            if (hipDocksManager.TryIsDocked(gameObject, out HipDocksManager.DockID dockID))
            {
                hipDocksManager.UndockWeapon(gameObject);
            }

            ShowHomeHUD(false);
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
        while (homeHUD.AmmoCount > 0)
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

        if (!homeHUD.HasAmmo) return;

        animator.SetTrigger("Fire");
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);

        if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
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

        homeHUD.DecrementAmmoCount();
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

        hudContainerManager.HUDManager.HideHUD();

        foreach(SocketCompatibilityLayerManager manager in socketCompatibilityLayerManagers)
        {
            if (TryGet.TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
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

    public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");

        if (!IsHeld) return;
        
        // if ((int) characteristics == (int) HandController.LeftHand)
        // {
        //     Log($"{Time.time} {gameObject.name} {className} OnActuation:Left Hand Actuation : {actuation}");
        // }
        // else if ((int) characteristics == (int) HandController.RightHand)
        // {
        //     Log($"{Time.time} {gameObject.name} {className} OnActuation:Right Hand Actuation : {actuation}");
        // }

        bool actuate = (Time.unscaledTime > lastActuation + actuationDelay);

        if (actuate)
        {
            if (actuation.HasFlag(Actuation.Thumbstick_Left) || actuation.HasFlag(Actuation.Thumbstick_Right))
            {
                HandleUINagivation(actuation);
            }

            if (enableQuickHome && actuation.HasFlag(Actuation.Thumbstick_Down) && !IsHUDShown((int) Interactables.Gun.HUDManager.Identity.Home))
            {
                ShowHUD((int) Interactables.Gun.HUDManager.Identity.Home);
            }

            lastActuation = Time.unscaledTime;
        }

        hudContainerManager.HUDManager.OnActuation(actuation, characteristics);
    }

    public void OnRawData(HandController.RawData rawData)
    {
        Log($"{Time.time} {gameObject.name} {className} OnRawData:RawData : {rawData}");

        if (!IsHeld) return;

        // TODO
    }

    private void HandleUINagivation(Actuation actuation)
    {
        Log($"{Time.time} {gameObject.name} {className} HandleUINagivation:Actuation : {actuation}");

        int idx = default(int);

        if ((actuation.HasFlag(Actuation.Thumbstick_Left) && (hudsManager.TryGetPreviousIndex(out idx))) || (actuation.HasFlag(Actuation.Thumbstick_Right) && (hudsManager.TryGetNextIndex(out idx))))
        {
            ShowHUD(idx);
        }
    }

    public void OnSocketCompatiblityLayerEvent(SocketCompatibilityLayerManager manager, SocketCompatibilityLayerManager.EventType eventType, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatiblityLayer:Manager : {manager.name} GameObject : {gameObject.name} Type : {eventType}");

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

    public bool TryGetCompatibleLayer(string tag, out SocketCompatibilityLayerManager socketCompatibilityLayerManager)
    {
        Log($"{Time.time} {gameObject.name} {className} TryGetCompatibleLayer:Tag : {tag}");

        SocketCompatibilityLayerManager manager = null;

        if (tag.Equals("Compact Flashlight"))
        {
            manager = socketCompatibilityLayerManagers.Where(s => s.CompareTag("Flashlight Socket")).FirstOrDefault();
        }
        else if (tag.Equals("Laser Sight"))
        {
            manager = socketCompatibilityLayerManagers.Where(s => s.CompareTag("Laser Sight Socket")).FirstOrDefault();
        }

        socketCompatibilityLayerManager = manager;
        return (manager != null);
    }

    private void HandleHoldingState(bool isTrue, IInteractable obj)
    {
        Log($"{Time.time} {this.gameObject.name} {className}.HandleHoldingState:IsTrue: {isTrue} GameObject : {obj.GetGameObject().name}");
        
        var gameObject = obj.GetGameObject();

        if (TryGetCompatibleLayer(gameObject.tag, out SocketCompatibilityLayerManager socketCompatibilityLayerManager))
        {
            if (TryGet.TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
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
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerEntryEvent:Manager : {manager.name} GameObject : {gameObject.name}");

        if (TryGet.TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
        {
            if (socketInteractorManager.IsOccupied) return;

            if (gameObject.CompareTag("Compact Flashlight"))
            {
                EnableSocketCheck(socketInteractorManager, gameObject, new string[] { "Default", "Gun Compatible Flashlight" });
            }
            else if (gameObject.CompareTag("Laser Sight"))
            {
                EnableSocketCheck(socketInteractorManager, gameObject, new string[] { "Default", "Gun Compatible Laser Sight" });
            }
        }
    }

    private void EnableSocketCheck(SocketInteractorManager manager, GameObject gameObject, string[] layerNames)
    {
        var layers = new StringBuilder();

        foreach (string value in layerNames)
        {
            layers.Append(((layers.Length == 0) ? $"[{value}]" : $" [{value}]"));
        }

        Log($"{Time.time} {this.gameObject.name}.EnableSocketCheck:Manager : {manager.gameObject.name} GameObject : {gameObject.name} LayerNames : {layers.ToString()}");

        if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
        {
            if (TryGet.TryGetOpposingController(controller, out HandController opposingController))
            {
                if ((opposingController.IsHolding) && (GameObject.ReferenceEquals(opposingController.Interactable.GetGameObject(), gameObject)))
                {
                    EnableSocket(manager, gameObject, layerNames);
                }
            }
        }
    }

    private void EnableSocket(SocketInteractorManager manager, GameObject gameObject, string[] layerNames)
    {
        var layers = new StringBuilder();

        foreach (string value in layerNames)
        {
            layers.Append(((layers.Length == 0) ? $"[{value}]" : $" [{value}]"));
        }

        Log($"{Time.time} {this.gameObject.name}.EnableSocket:Manager : {manager.gameObject.name} GameObject : {gameObject.name} LayerNames : {layers.ToString()}");
        
        manager.EnablePreview(true);

        if (gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
        {
            interactable.interactionLayers = InteractionLayerMask.GetMask(layerNames);
            manager.EnableSocket(true);
        }
    }

    private void DisableSocketCheck(SocketInteractorManager manager, GameObject gameObject, string[] layerNames)
    {
        var layers = new StringBuilder();

        foreach (string value in layerNames)
        {
            layers.Append(((layers.Length == 0) ? $"[{value}]" : $" [{value}]"));
        }

        Log($"{Time.time} {this.gameObject.name}.DisableSocketCheck:Manager : {manager.gameObject.name} GameObject : {gameObject.name} LayerNames : {layers.ToString()}");

        if (TryGet.TryGetIdentifyController(interactor, out HandController controller))
        {
            if (TryGet.TryGetOpposingController(controller, out HandController opposingController))
            {
                if ((opposingController.IsHolding) && (GameObject.ReferenceEquals(opposingController.Interactable.GetGameObject(), gameObject)))
                {
                    DisableSocket(manager, gameObject, layerNames);
                }
            }
        }
    }

    private void DisableSocket(SocketInteractorManager manager, GameObject gameObject, string[] layerNames)
    {
        var layers = new StringBuilder();

        foreach (string value in layerNames)
        {
            layers.Append(((layers.Length == 0) ? $"[{value}]" : $" [{value}]"));
        }

        Log($"{Time.time} {this.gameObject.name}.DisableSocket:Manager : {manager.gameObject.name} GameObject : {gameObject.name} LayerNames : {layers.ToString()}");
        
        manager.EnablePreview(false);

        if (gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable interactable))
        {
            manager.EnableSocket(false);
            interactable.interactionLayers = InteractionLayerMask.GetMask(layerNames);
        }
    }

    private void OnSocketHoverEntryEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketHoverEntryEvent:GameObject : {gameObject.name}");
    }

    private void OnSocketSelectEntryEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketSelectEntryEvent:GameObject : {gameObject.name}");

        int? id = null;

        if (gameObject.CompareTag("Compact Flashlight"))
        {
            if (gameObject.TryGetComponent<FlashlightInteractableManager>(out var flashlightManager))
            {
                var hudID = Interactables.Gun.HUDManager.Identity.Flashlight;
                id = (int) hudID;

                hudsManager.SetActive((int) id);

                if (hudContainerManager.TryGetHUDManagerById(hudID, out Interactables.Gun.HUDManager hudManager))
                {
                    if (flashlightManager.State == FlashlightInteractableManager.ActiveState.On)
                    {
                        ((Interactables.Gun.FlashlightHUDManager) hudManager).SetStateNoAudio(Enum.GunInteractableEnums.State.Active);
                    }
                    else
                    {
                        ((Interactables.Gun.FlashlightHUDManager) hudManager).SetStateNoAudio(Enum.GunInteractableEnums.State.Inactive);
                    }
                }
            }
        }
        else if (gameObject.CompareTag("Laser Sight"))
        {
            if (gameObject.TryGetComponent<LaserSightInteractableManager>(out var laserSightManager))
            {
                var hudID = Interactables.Gun.HUDManager.Identity.LaserSight;
                id = (int) hudID;

                hudsManager.SetActive((int) id);
                
                if (hudContainerManager.TryGetHUDManagerById(hudID, out Interactables.Gun.HUDManager hudManager))
                {
                    if (laserSightManager.State == LaserSightInteractableManager.ActiveState.On)
                    {
                        ((Interactables.Gun.LaserSightHUDManager) hudManager).SetStateNoAudio(Enum.GunInteractableEnums.State.Active);
                    }
                    else
                    {
                        ((Interactables.Gun.LaserSightHUDManager) hudManager).SetStateNoAudio(Enum.GunInteractableEnums.State.Inactive);
                    }
                }
            }
        }

        if (switchHUDOnDock && id.HasValue)
        {
            AudioSource.PlayClipAtPoint(navigationClip, transform.position, 1.0f);
            ShowHUD((int) id.Value);
        }
    }

    private void OnSocketSelectExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketSelectExitEvent:GameObject : {gameObject.name}");

        int? id = null;

        if (gameObject.TryGetComponent<FlashlightInteractableManager>(out var flashlightManager))
        {
            var hudID = Interactables.Gun.HUDManager.Identity.Flashlight;
            id = (int) hudID;

            if (hudContainerManager.TryGetHUDManagerById(hudID, out Interactables.Gun.HUDManager hudManager))
            {
                ((Interactables.Gun.FlashlightHUDManager) hudManager).SetState(Enum.GunInteractableEnums.State.Inactive);
            }
        }
        else if (gameObject.TryGetComponent<LaserSightInteractableManager>(out var laserSightManager))
        {
            var hudID = Interactables.Gun.HUDManager.Identity.LaserSight;
            id = (int) hudID;

            if (hudContainerManager.TryGetHUDManagerById(hudID, out Interactables.Gun.HUDManager hudManager))
            {
                ((Interactables.Gun.LaserSightHUDManager) hudManager).SetState(Enum.GunInteractableEnums.State.Inactive);
            }
        }

        if (id.HasValue)
        {
            hudsManager.SetInactive((int) id.Value);
        }

        ShowHomeHUD(false);
    }

    private bool IsHUDShown(int id)
    {
        return hudContainerManager.IsHUDShown(id);
    }

    private void ShowHomeHUD(bool audible = true)
    {
        if (audible)
        {
            AudioSource.PlayClipAtPoint(navigationClip, transform.position, 1.0f);
        }

        hudContainerManager.ShowHomeHUD();
    }

    private void ShowHUD(int id, bool audible = true)
    {
        if (audible)
        {
            AudioSource.PlayClipAtPoint(navigationClip, transform.position, 1.0f);
        }

        hudContainerManager.ShowHUD((int) id);
    }

    private void OnSocketHoverExitEvent(GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketHoverExitEvent:GameObject : {gameObject.name}");
    }

    private void OnSocketCompatibilityLayerExitEvent(SocketCompatibilityLayerManager manager, GameObject gameObject)
    {
        Log($"{Time.time} {this.gameObject.name}.OnSocketCompatibilityLayerExitEvent:Manager : {manager.name} GameObject : {gameObject.name}");

        if (TryGet.TryGetSocketInteractorManager(manager, out SocketInteractorManager socketInteractorManager))
        {
            if (gameObject.CompareTag("Compact Flashlight"))
            {
                DisableSocketCheck(socketInteractorManager, gameObject, new string[] { "Default", "Flashlight" });
            }
            else if (gameObject.CompareTag("Laser Sight"))
            {
                DisableSocketCheck(socketInteractorManager, gameObject, new string[] { "Default", "Laser Sight" });
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

    public void RestoreAmmoCount() => homeHUD.RestoreAmmoCount();
}