using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketInteractorManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Serializable]
    public class OccupancyData
    {
        public bool occupied;
        public GameObject gameObject;
    }

    public enum EventType
    {
        OnEntry,
        OnHovering,
        OnDocked,
        OnUndocked,
        OnExitHovering,
        OnExit
    }

    [SerializeField] MeshRenderer visualElement;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    [Header("XR Socket")]
    [SerializeField] new SphereCollider collider;

    [Header("Optional settings")]
    [SerializeField] bool startEnabled = true;

    public delegate void Event(SocketInteractorManager manager, EventType type, GameObject gameObject);
    public event Event EventReceived;

    public OccupancyData Data { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }
    public bool IsOccupied { get { return occupied != null && occupied.occupied; } }
    public void Free() => occupied = new SocketInteractorManager.OccupancyData();

    private XRSocketInteractor socketInteractor;
    private Transform objects;
    private bool canDock, isDocked;

    void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        Reveal(startEnabled);
    }

    private void ResolveDependencies()
    {
        socketInteractor = GetComponent<XRSocketInteractor>() as XRSocketInteractor;
    }

    public void Reveal(bool reveal)
    {
        Log($"{Time.time} {gameObject.name} {className} Reveal:{reveal}");
        visualElement.enabled = reveal;
    }

    public void EnableCollider(bool enable)
    {
        Log($"{Time.time} {gameObject.name} {className} EnableCollider:{enable}");
        collider.enabled = enable;
    }

    public void OnTriggerEnter(Collider collider)
    {
        var trigger = collider.gameObject;
        Log($"{Time.time} {gameObject.name} {className} OnTriggerEnter:GameObject : {trigger.name}");

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnEntry, trigger);
        }
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered");
        var interactableGameObject = args.interactableObject.transform.gameObject;

        visualElement.gameObject.SetActive(false);

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnHovering, interactableGameObject);
        }
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered");
        var interactableGameObject = args.interactableObject.transform.gameObject;
        visualElement.enabled = false;

        Data = new OccupancyData
        {
            occupied = true,
            gameObject = interactableGameObject
        };

        AudioSource.PlayClipAtPoint(dockClip, transform.position, 1.0f);
        isDocked = true;

        if (interactableGameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Log($"{Time.time} {gameObject.name} {className} Prep OnDockStatusChange");
            interactable.OnDockStatusChange(isDocked);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnDocked, interactableGameObject);
        }
    }

     public void OnSelectExited(SelectExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited");

        var interactableGameObject = args.interactableObject.transform.gameObject;
        visualElement.enabled = true;

        Data = new OccupancyData
        {
            occupied = false,
            gameObject = null
        };

        AudioSource.PlayClipAtPoint(undockClip, transform.position, 1.0f);

        isDocked = false;

        if (interactableGameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnDockStatusChange(isDocked);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnUndocked, interactableGameObject);
        }
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverExited");
        var interactableGameObject = args.interactableObject.transform.gameObject;

        visualElement.gameObject.SetActive(true);

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnExit, interactableGameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var trigger = collider.gameObject;
        Log($"{Time.time} {gameObject.name} {className} OnTriggerExit:GameObject : {trigger.name}");

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnExit, trigger);
        }
    }
}