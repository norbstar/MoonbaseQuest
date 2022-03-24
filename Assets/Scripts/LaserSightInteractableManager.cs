using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
public class LaserSightInteractableManager : FocusableInteractableManager, IActuation
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public enum ActiveState
    {
        Off,
        On
    }

    [Header("Components")]
    [SerializeField] GameObject laser;

    [Header("Audio")]
    [SerializeField] AudioClip buttonClip;

    [Header("Config")]
    [SerializeField] ActiveState startState;

    [Header("References")]
    [SerializeField] GameObject spawnPoint;

    [Header("Prefabs")]

    [SerializeField] GameObject pointPrefab;

    private ActiveState state;
    private GameObject pointPrefabInstance;

    public ActiveState State {
        get
        {
            return state;
        }

        set
        {
            state = value;
            laser.SetActive(state == ActiveState.On);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        State = startState;
        laser.SetActive(state == ActiveState.On);
    }

    void FixedUpdate()
    {
        if (!laser.activeSelf)
        {
            pointPrefabInstance?.SetActive(false);
            return;
        }

        var ray = new Ray(spawnPoint.transform.position, spawnPoint.transform.up);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
        {
            var objectHit = hit.transform.gameObject;
            var point = hit.point;

            if (pointPrefabInstance == null)
            {
                pointPrefabInstance = Instantiate(pointPrefab, point, Quaternion.identity);
            }
            else
            {
                pointPrefabInstance.transform.position = point;
                pointPrefabInstance.SetActive(true);
            }
        }  
    }

    public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation} Value : {value}");

        if (!IsHeld) return;

        if (actuation.HasFlag(Actuation.Button_AX))
        {
            AlternateState();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited:Controller : {controller.name}");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.5f, transform.forward, 0f);

        bool isHovering = false;

        foreach (RaycastHit hit in hits)
        {
            GameObject gameObject = hit.collider.gameObject;
            
            if (gameObject.TryGetComponent<XRSocketInteractor>(out XRSocketInteractor interactor))
            {
                List<IXRHoverInteractable> interactables = interactor.interactablesHovered;

                foreach (IXRHoverInteractable interactable in interactables)
                {
                    if (GameObject.ReferenceEquals(interactable.transform.gameObject, this.interactable.gameObject))
                    {
                        isHovering = true;
                    }
                }
            }
        }

        if (!isHovering)
        {
            Log($"{Time.time} {gameObject.name} {className} OnSelectExited:Revert Interaction Mask");
            this.interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Laser Sight" });
        }
    }

    private void AlternateState()
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position, 1.0f);
        State = (state == ActiveState.Off) ? ActiveState.On : ActiveState.Off;
    }
}