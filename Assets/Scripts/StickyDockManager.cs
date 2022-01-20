using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickyDockManager : DockManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Restrictions")]
    [SerializeField] List<string> supportedTags;

    [Header("Tracking")]
    [SerializeField] bool showTrackers;
    [SerializeField] GameObject trackerPrefab;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private new Collider collider;
    private Transform objects;
    private GameObject trackerPrefabInstance;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<Collider>() as Collider;
    }

    void OnEnable()
    {
        InteractableManager.EventReceived += OnEvent;
    }

    private InteractableManager trackedInteractable;

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (TryGetInteractable<InteractableManager>(trigger, out InteractableManager interactable))
        {
            if (supportedTags.Contains(interactable.tag))
            {
                MarkTrackedObject(interactable);
            }
        }
    }

    private void MarkTrackedObject(InteractableManager interactable)
    {
        if (trackedInteractable != null)
        {
            trackedInteractable.HideDockSite();
        }

        trackedInteractable = interactable;
        trackedInteractable.ShowDockSite();
    }

#if false
    public void OnTriggerStay(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (!Object.ReferenceEquals(trigger, trackedInteractable.gameObject)) return;

#if false
        if (TryGetInteractable<InteractableManager>(trigger, out InteractableManager interactable))
        {
            if (supportedTags.Contains(interactable.tag))
            {
                // Log($"{gameObject.name} {className} OnTriggerStay.ID:{interactable.name}");
                // var adjustedPosition = GetAdjustedPosition(interactable);

                if (showTrackers)
                {
                    if (trackerPrefabInstance == null)
                    {
                        // trackerPrefabInstance = Instantiate(trackerPrefab, /*transform.TransformPoint*/(adjustedPosition), Quaternion.identity);
                        // trackerPrefabInstance = Instantiate(trackerPrefab, interactable.transform.position, Quaternion.identity);
                        // trackerPrefabInstance = Instantiate(trackerPrefab, adjustedPosition, Quaternion.identity);

                        // trackerPrefabInstance = Instantiate(trackerPrefab, interactable.transform.position, interactable.transform.localRotation);
                        // var point = trackerPrefabInstance.transform.GetChild(0);
                        // point.transform.localPosition = interactable.DockTransform.localPosition;
                        // trackerPrefabInstance.transform.localRotation = interactable.transform.localRotation;

                        trackerPrefabInstance = Instantiate(trackerPrefab);
                        trackerPrefabInstance.transform.parent = interactable.transform;
                        // trackerPrefabInstance.transform.rotation = interactable.transform.rotation;
                        // trackerPrefabInstance.transform.position += interactable.DockTransform.localPosition;
                        trackerPrefabInstance.transform.position = interactable.DockTransform.position;
                    }
                    else
                    {
                        // trackerPrefabInstance.transform.position = /*transform.TransformPoint*/(adjustedPosition);
                        // trackerPrefabInstance.transform.position = interactable.transform.position;
                        // trackerPrefabInstance.transform.position = adjustedPosition;

                        // trackerPrefabInstance.transform.position = interactable.transform.position;
                    }

                    Log($"{gameObject.name} {className} OnTriggerStay.Position:{trackerPrefabInstance.transform.position}");
                }
            }
        }
#endif
    }
#endif

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (Object.ReferenceEquals(trigger, trackedInteractable.gameObject))
        {
            trackedInteractable = null;
        }
    }

    private bool TryGetInteractable<InteractableManager>(GameObject trigger, out InteractableManager interactable)
    {
        if (trigger.TryGetComponent<InteractableManager>(out InteractableManager interactableManager))
        {
            interactable = interactableManager;
            return true;
        }

        var component = trigger.GetComponentInParent<InteractableManager>();

        if (component != null)
        {
            interactable = component;
            return true;
        }

        interactable = default(InteractableManager);
        return false;
    }

    void OnDisable()
    {
        InteractableManager.EventReceived -= OnEvent;
    }

    public void OnEvent(InteractableManager interactable, InteractableManager.EventType type)
    {
#if false
        switch (type)
        {
            case InteractableManager.EventType.OnSelectEntered:
                Log($"{Time.time} {gameObject.name} {className} OnEvent.OnSelectEntered");
                if (Object.ReferenceEquals(Data.gameObject, interactable.gameObject))
                {
                    UndockInteractable();
                }

                break;

            case InteractableManager.EventType.OnSelectExited:
                Log($"{Time.time} {gameObject.name} {className} OnEvent.OnSelectExited");
                if (Data.occupied) return;

                Log($"{Time.time} {gameObject.name} {className} OnEvent.OnSelectEntered.Is Not Occupied");
                
                Debug.Log($"{Time.time} {gameObject.name} {className} Position: {interactable.transform.position}");
                var adjustedPosition = GetAdjustedPosition(interactable);
                Debug.Log($"{Time.time} {gameObject.name} {className} Adjusted Position: {adjustedPosition}");
                if ((collider.bounds.Contains(adjustedPosition)) && (supportedTags.Contains(interactable.tag)))
                {
                    DockInteractable(interactable);
                }
                break;
        }
#endif
    }

    private void DockInteractable(InteractableManager interactable)
    {
        Log($"{Time.time} {gameObject.name} {className} OnEvent.DockInteractable");

        DampenVelocity(interactable.gameObject);

        interactable.transform.parent = transform;

        var adjustedPosition = GetAdjustedPosition(interactable);
        interactable.transform.position = adjustedPosition;

        Data = new OccupancyData
        {
            occupied = true,
            gameObject = interactable.gameObject
        };
    }

    private Vector3 GetAdjustedPosition(InteractableManager interactable)
    {
        var dockTransform = interactable.DockTransform;
        // return (dockTransform != null) ? Vector3.zero - (dockTransform.position * (dockTransform.localScale.x / transform.localScale.x)) : Vector3.zero;
        return (dockTransform != null) ? interactable.transform.position - (dockTransform.position * (dockTransform.localScale.x / transform.localScale.x)) : interactable.transform.position;
        // return (dockTransform != null) ? interactable.transform.position - dockTransform.position : interactable.transform.position;
    }

    private Quaternion GetAdjustedRotation(InteractableManager interactable)
    {
        var dockTransform = interactable.DockTransform;
        return (dockTransform != null) ? Quaternion.identity * Quaternion.Inverse(dockTransform.rotation) : Quaternion.identity;
    }

    private void DampenVelocity(GameObject gameObject)
    {
        Log($"{Time.time} {gameObject.name} {className} OnEvent.DampenVelocity");

        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
        }
    }

    private void UndockInteractable()
    {
        Log($"{Time.time} {gameObject.name} {className} OnEvent.UndockInteractable");

        Data = new OccupancyData
        {
            occupied = false,
            gameObject = null
        };
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}