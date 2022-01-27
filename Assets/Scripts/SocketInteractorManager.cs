using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketInteractorManager : MonoBehaviour
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
        OnDocked,
        OnUndocked
    }

    [SerializeField] MeshRenderer visualElement;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public delegate void Event(SocketInteractorManager manager, EventType type, GameObject gameObject);
    public event Event EventReceived;

    public OccupancyData Data { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }

    public void Free() => occupied = new SocketInteractorManager.OccupancyData();

    private Transform objects;
    private bool canDock, isDocked;

    void Awake()
    {
        objects = GameObject.Find("Objects").transform;
    }

     public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered");

        visualElement.gameObject.SetActive(false);
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverExited");

        visualElement.gameObject.SetActive(true);
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

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}