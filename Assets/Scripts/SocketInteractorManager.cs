using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
[RequireComponent(typeof(SphereCollider))]
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
        OnHoverEntered,
        OnSelectEntered,
        OnSelectExited,
        OnHoverExited
    }

    [SerializeField] MeshRenderer visualElement;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    [Header("Optional settings")]
    [SerializeField] bool startEnabled = true;

    public delegate void Event(SocketInteractorManager manager, EventType type, GameObject gameObject);
    public event Event EventReceived;

    public OccupancyData Data { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }
    public bool IsOccupied { get { return occupied != null && occupied.occupied; } }
    public void Free() => occupied = new SocketInteractorManager.OccupancyData();

    private XRSocketInteractor socketInteractor;
    private Transform objects;

    void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        EnablePreview(startEnabled);
    }

    private void ResolveDependencies()
    {
        socketInteractor = GetComponent<XRSocketInteractor>() as XRSocketInteractor;
    }

    public void EnablePreview(bool enable)
    {
        Log($"{Time.time} {gameObject.name} {className} EnablePreview:{enable}");
        visualElement.enabled = enable;
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered");
        var interactableGameObject = args.interactableObject.transform.gameObject;
        visualElement.gameObject.SetActive(false);

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnHoverEntered, interactableGameObject);
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

        if (interactableGameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Log($"{Time.time} {gameObject.name} {className} Prep OnDockStatusChange");
            interactable.OnDockStatusChange(Data.occupied);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectEntered, interactableGameObject);
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

        if (interactableGameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnDockStatusChange(Data.occupied);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnSelectExited, interactableGameObject);
        }
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverExited");
        var interactableGameObject = args.interactableObject.transform.gameObject;

        visualElement.gameObject.SetActive(true);

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnHoverExited, interactableGameObject);
        }
    }
}