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

    protected GameObject interactor;
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
        interactor = args.interactableObject.transform.gameObject;
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered:Interactor Object : {args.interactorObject} Interactor : {interactor}");
        visualElement.enabled = false;

        Data = new OccupancyData
        {
            occupied = true,
            gameObject = interactor
        };

        isDocked = true;

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnDocked, interactor);
        }
    }

     public void OnSelectExited(SelectExitEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited");
        interactor = args.interactableObject.transform.gameObject;
        visualElement.enabled = true;

        Data = new OccupancyData
        {
            occupied = false,
            gameObject = null
        };

        isDocked = false;

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnUndocked, interactor);
        }
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}