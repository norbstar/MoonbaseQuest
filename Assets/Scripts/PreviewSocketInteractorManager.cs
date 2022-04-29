using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
[RequireComponent(typeof(SphereCollider))]
public class PreviewSocketInteractorManager : BaseManager
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

    [Header("Mesh")]
    [SerializeField] GameObject mesh;

    [Header("Attach Transform")]
    [SerializeField] new Transform transform;

    [Header("Audio")]
    [SerializeField] AudioClip dockClip;
    [SerializeField] AudioClip undockClip;

    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    [Header("Optional settings")]
    [SerializeField] bool startEnabled = true;

    public delegate void Event(PreviewSocketInteractorManager manager, EventType type, GameObject gameObject);
    public event Event EventReceived;

    public OccupancyData Data { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }
    public bool IsOccupied { get { return occupied != null && occupied.occupied; } }
    public void Free() => occupied = new PreviewSocketInteractorManager.OccupancyData();

    private XRSocketInteractor socketInteractor;
    private Transform objects;
    private MeshFilter meshFilter;

    void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        EnablePreview(startEnabled);
    }

    private void ResolveDependencies()
    {
        socketInteractor = GetComponent<XRSocketInteractor>() as XRSocketInteractor;
        meshFilter = mesh.GetComponent<MeshFilter>() as MeshFilter;
    }

    public void EnablePreview(bool enable) => mesh.SetActive(enable);

    public void EnableSocket(bool enable) => socketInteractor.socketActive = enable;

    private void CalibrateAttachTransform(GameObject gameObject)
    {
        Debug.Log($"CalibrateAttachTransform");

        var interactable = gameObject.GetComponent<XRGrabInteractable>() as XRGrabInteractable;

        transform.localPosition = new Vector3(interactable.attachTransform.localPosition.x, interactable.attachTransform.localPosition.y - 0.5f, interactable.attachTransform.localPosition.z);
        transform.localRotation = interactable.attachTransform.localRotation;
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnHoverEntered");

        var interactableGameObject = args.interactableObject.transform.gameObject;
        CalibrateAttachTransform(interactableGameObject);

        if (!IsOccupied)
        {
            meshFilter.mesh = interactableGameObject.GetComponent<MeshFilter>().mesh;
            meshFilter.gameObject.SetActive(true);
        }

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnHoverEntered, interactableGameObject);
        }
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectEntered");

        var interactableGameObject = args.interactableObject.transform.gameObject;
        CalibrateAttachTransform(interactableGameObject);

        meshFilter.gameObject.SetActive(false);

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
        
        meshFilter.gameObject.SetActive(true);

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

        meshFilter.gameObject.SetActive(false);
        meshFilter.mesh = null;

        if (EventReceived != null)
        {
            EventReceived(this, EventType.OnHoverExited, interactableGameObject);
        }
    }
}