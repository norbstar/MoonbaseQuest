using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusableInteractableManager, IGesture
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public enum Mode
    {
        Manual,
        Auto
    }

    public enum Intent
    {
        Engaged,
        Disengaged
    }

    public enum State
    {
        Active,
        Inactive
    }

    public static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    public static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    [SerializeField] new Camera camera;
    [SerializeField] Animator animator;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject laserPrefab, laserFXPrefab;
    [SerializeField] GunHUDCanvasManager hudCanvasManager;
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;
    [SerializeField] float speed = 5f;

    [Header("Config")]
    [SerializeField] bool autoDockEnabled = true;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Laser")]
    [SerializeField] bool spawnLaser;

    [Header("Over Load")]
    [SerializeField] float overLoadThreshold;

    [Header("Docking")]
    [SerializeField] StickyDockManager stickyDock;

    [Header("Audio")]
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip manualClip;
    [SerializeField] AudioClip autoClip;
    [SerializeField] AudioClip overloadedClip;
    [SerializeField] AudioClip engagedClip;
    [SerializeField] AudioClip disengagedClip;

    private CurveCreator curveCreator;
    private MainCameraManager mainCameraManager;
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
    private Coroutine fireRepeatCoroutine;
    private float heat;
    private IList<float> heatValues;
    private int heatIndex;
    private State state;
    private bool dockedOccupied;
    private GameObject docked;

    protected override void Awake()
    {
        base.Awake();
        
        ResolveDependencies();
        CacheGunState();

        stickyDock.EventReceived += OnDockEvent;

        mixedLayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Asteroid Layer");
        overheatCanvasManager.SetMaxValue(overLoadThreshold);
        heatValues = curveCreator.Values;

        StartCoroutine(ManageHeatCoroutine());
    }

    private void ResolveDependencies()
    {
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
        mainCameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;
        hipDocksManager = mainCameraManager.HipDocksManager;
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
            
            var opposingController = mainCameraManager.GetOppositeHandController(controller);

            if (opposingController.IsHolding)
            {
                var interactable = opposingController.Interactable;
                
                if (interactable.CompareTag("Flashlight"))
                {
                    stickyDock.gameObject.SetActive(true);
                }
            }    
        }

        if (stickyDock.Data.occupied)
        {
            stickyDock.Data.gameObject.GetComponent<XRGrabInteractable>().enabled = true;
            stickyDock.GetComponent<MeshRenderer>().enabled = true;
        }
            
        InteractableManager.EventReceived += OnEvent;
    }

    public void OnActivated(ActivateEventArgs args)
    {
        if (mode == Mode.Manual)
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

        if (autoDockEnabled && (controller != null))
        {
            DockWeapon(controller);
        }

        if (!dockedOccupied)
        {
            stickyDock.gameObject.SetActive(false);
        }
        else
        {
            stickyDock.GetComponent<MeshRenderer>().enabled = false;
        }

        if (stickyDock.Data.occupied)
        {
            stickyDock.Data.gameObject.GetComponent<XRGrabInteractable>().enabled = false;
        }

        InteractableManager.EventReceived -= OnEvent;
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
        if (!stickyDock.Data.occupied) return;

        Log($"{gameObject.name} {className} Intent: {intent}");

        var dockedObject = stickyDock.Data.gameObject;
        FlashlightInteractableManager manager = dockedObject.GetComponent<FlashlightInteractableManager>() as FlashlightInteractableManager;

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

    public void OnEvent(InteractableManager interactable, EventType eventType)
    {
        Log($"{this.gameObject.name}.OnEvent:[{interactable.name}]:Type : {eventType}");

        switch (eventType)
        {
            case EventType.OnSelectEntered:
                if (IsHeld && interactable.CompareTag("Flashlight"))
                {
                    stickyDock.gameObject.SetActive(true);
                }
                break;

            case EventType.OnSelectExited:
                if (IsHeld && interactable.CompareTag("Flashlight"))
                {
                    if (!dockedOccupied)
                    {
                        stickyDock.gameObject.SetActive(false);
                    }
                }
                break;
        }
    }

    public void OnDockEvent(StickyDockManager manager, StickyDockManager.EventType eventType, GameObject gameObject)
    {
        Log($"{this.gameObject.name}.OnDockEvent:[{gameObject.name}]:Type : {eventType}");

        switch (eventType)
        {
            case StickyDockManager.EventType.OnDocked:
                stickyDock.gameObject.SetActive(true);
                
                FlashlightInteractableManager flashlightManager = gameObject.GetComponent<FlashlightInteractableManager>() as FlashlightInteractableManager;

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
                
                dockedOccupied = true;
                docked = gameObject;
                break;

            case StickyDockManager.EventType.OnUndocked:
                hudCanvasManager.SetIntent(Intent.Disengaged);
                this.intent = Intent.Disengaged;
                
                dockedOccupied = false;
                docked = null;
                break;
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