using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(CurveCreator))]
public class GunInteractableManager : FocusManager, IGesture
{
    public enum Mode
    {
        Manual,
        Auto
    }

    public enum State
    {
        Active,
        Inactive
    }

    private static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    private static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    [SerializeField] new Camera camera;
    [SerializeField] IdentityCanvasManager identityCanvasManager;
    [SerializeField] Animator animator;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject laserPrefab, laserFXPrefab;
    [SerializeField] GunAmmoCanvasManager ammoCanvasManager;
    [SerializeField] GunOverheatCanvasManager overheatCanvasManager;
    [SerializeField] float speed = 5f;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Laser")]
    [SerializeField] bool spawnLaser;

    [Header("Over Load")]
    [SerializeField] float overLoadThreshold;

    [Header("Audio")]
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip manualClip;
    [SerializeField] AudioClip autoClip;
    [SerializeField] AudioClip overloadedClip;

    public bool IsHeld { get { return isHeld; } }

    private XRGrabInteractable interactable;
    private CurveCreator curveCreator;
    private MainCameraManager cameraManager;
    private GameObject lastEngageable;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private GameObject lastObjectHit;
    private IFocus lastFocus;
    private GameObject hitPrefabInstance;
    private int mixedLayerMask;
    private bool isHeld;
    private Mode mode;
    private Coroutine fireRepeatCoroutine;
    private float heat;
    private IList<float> heatValues;
    private int heatIndex;
    private State state;
    private Transform guns;

    void Awake()
    {
        ResolveDependencies();
        CacheGunState();

        mixedLayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Asteroid Layer");
        identityCanvasManager.IdentityText = gameObject.name;
        overheatCanvasManager.SetMaxValue(overLoadThreshold);
        heatValues = curveCreator.Values;
        guns = GameObject.Find("Objects/Guns").transform;

        StartCoroutine(ManageHeatCoroutine());
    }

    private void ResolveDependencies()
    {
        interactable = GetComponent<XRGrabInteractable>() as XRGrabInteractable;
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
        cameraManager = camera.GetComponent<MainCameraManager>() as MainCameraManager;
    }

    void FixedUpdate()
    {
        var ray = new Ray(spawnPoint.transform.position, spawnPoint.transform.forward);
        // Debug.DrawLine(ray.origin, ray.GetPoint(10f), Color.blue, 0.1f);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, mixedLayerMask))
        {
            var objectHit = hit.transform.gameObject;
            // Debug.Log($"{gameObject.name}.Hit:{objectHit.name}");

            var point = hit.point;
            // Debug.Log($"{gameObject.name}.Point:{point}");

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

                if (TryGetController<HandController>(out HandController controller))
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
        }
    }

    protected override void OnFocusGained()
    {
        identityCanvasManager.gameObject.SetActive(true);
    }

    protected override void OnFocusLost()
    {
        identityCanvasManager.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider collider)
    {
        // Debug.Log($"{gameObject.name}:On Trigger Enter : {collider.gameObject.tag}");
        
        if (collider.gameObject.CompareTag("Hand"))
        {
            lastEngageable = collider.gameObject;
        }
    }

    public void OnSelectEntered()
    {
        // Debug.Log($"{gameObject.name}:On Select Entered");

        if (TryGetController<HandController>(out HandController controller))
        {
            controller.SetHolding(gameObject);
            gameObject.transform.parent = guns;
            isHeld = true;

            var device = controller.GetInputDevice();

            if (((int) device.characteristics) == ((int) LeftHand))
            {
                ammoCanvasManager.transform.localPosition = new Vector3(-0.05f, 0.05f, 0f);
                cameraManager.MarkDock(MainCameraManager.DockID.Left, false);
            }
            else if (((int) device.characteristics) == ((int) RightHand))
            {
                ammoCanvasManager.transform.localPosition = new Vector3(0.05f, 0.05f, 0f);
                cameraManager.MarkDock(MainCameraManager.DockID.Right, false);
            }
    
            ammoCanvasManager.gameObject.SetActive(true);
        }
    }

    public void OnActivated()
    {
        // Debug.Log($"{gameObject.name}:On Activated");

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
        while (ammoCanvasManager.AmmoCount > 0)
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

        if (ammoCanvasManager.AmmoCount == 0) return;

        animator.SetTrigger("Fire");
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);

        var controller = lastEngageable.GetComponent<HandController>() as HandController;
        controller?.SetImpulse();

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
            interactableEvent.OnActivate(interactable);
        }

        ammoCanvasManager.DecrementAmmoCount();
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

    public void OnDeactivated()
    {
        // Debug.Log($"{gameObject.name}:On Deactivated");

        if (lastEngageable.CompareTag("Hand"))
        {
            if (fireRepeatCoroutine != null)
            {
                StopCoroutine(fireRepeatCoroutine);
            }
        }
    }

    public void OnSelectExited()
    {
        // Debug.Log($"{gameObject.name}:On Select Exited");

        if (TryGetController<HandController>(out HandController controller))
        {
            ammoCanvasManager.gameObject.SetActive(false);
            DockWeapon(controller);
            controller.SetHolding(null);
            isHeld = false;
        }
    }

    private void DockWeapon(HandController controller)
    {
        var device = controller.GetInputDevice();

        if (((int) device.characteristics) == ((int) LeftHand))
        {
            cameraManager.DockObject(gameObject, MainCameraManager.DockID.Left);
        }
        else if (((int) device.characteristics) == ((int) RightHand))
        {
            cameraManager.DockObject(gameObject, MainCameraManager.DockID.Right);
        }
    }

    private bool TryGetController<HandController>(out HandController controller)
    {
        if (lastEngageable != null && lastEngageable.CompareTag("Hand"))
        {
            if (lastEngageable.TryGetComponent<HandController>(out HandController handController))
            {
                controller = handController;
                return true;
            }
        }

        controller = default(HandController);
        return false;
    }

    public void OnGesture(HandController.Gesture gesture, object value = null)
    {
        // Debug.Log($"{gameObject.name} On Gesture:Gesture : {gesture}");

        switch (gesture)
        {
            case HandController.Gesture.ThumbStick_Left:
                SetMode(Mode.Manual);
                break;
            
            case HandController.Gesture.ThumbStick_Right:
                SetMode(Mode.Auto);
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
                    ammoCanvasManager.SetMode(mode);
                    this.mode = mode;
                }
                break;

            case Mode.Auto:
                if (this.mode != Mode.Auto)
                {
                    AudioSource.PlayClipAtPoint(autoClip, transform.position, 1.0f);
                    ammoCanvasManager.SetMode(mode);
                    this.mode = mode;
                }
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

    public void RestoreAmmoCount() => ammoCanvasManager.RestoreAmmoCount();
}