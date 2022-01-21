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

    [Header("Compensation")]
    [SerializeField] Vector3 adjustedRotation;

    [Header("Materials")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material highlightMaterial;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private new Collider collider;
    private new MeshRenderer renderer;
    private Transform objects;
    private GameObject trackerPrefabInstance;
    private bool canDock, isDocked;

    // public Collider Colliders { get { return interactable.colliders; } }

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<Collider>() as Collider;
        renderer = GetComponent<MeshRenderer>() as MeshRenderer;
    }

    void OnEnable()
    {
        InteractableManager.EventReceived += OnEvent;
    }

    void FixedUpdate()
    {
        if (trackedInteractable == null)
        {
            HighlightDock(false);
            return;
        }

        if (isDocked) return;

         if ((collider.bounds.Contains(trackedInteractable.OriginTransform.position)) && (supportedTags.Contains(trackedInteractable.tag)))
         {
             HighlightDock(true);
             canDock = true;
         }
         else
         {
             HighlightDock(false);
             canDock = false;
         }
    }

    private void HighlightDock(bool enable)
    {
        renderer.material = (enable) ? highlightMaterial : defaultMaterial;
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
        if (trackedInteractable == null)
        {
            trackedInteractable = interactable;
            trackedInteractable.ShowTrackingVolume();
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (trackedInteractable == null) return;

        if (TryGetInteractable<InteractableManager>(trigger, out InteractableManager interactable))
        {
            if (Object.ReferenceEquals(interactable.gameObject, trackedInteractable.gameObject))
            {
                var colliders = interactable.Colliders;

                bool verified = true;

                foreach (Collider thisCollider in colliders)
                {
                    if ((thisCollider != collider) && (thisCollider.bounds.Intersects(this.collider.bounds)))
                    {
                        verified = false;
                    }
                }
                    
                if (verified)
                {
                    trackedInteractable.HideTrackingVolume();
                    trackedInteractable = null;
                }
            }
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

                if (canDock)
                {
                    DockInteractable(interactable);
                }
                break;
        }
    }

    private void DockInteractable(InteractableManager interactable)
    {
        Log($"{Time.time} {gameObject.name} {className} OnEvent.DockInteractable");

        DampenVelocity(interactable.gameObject);

        interactable.transform.SetParent(transform, true);
        interactable.transform.localPosition = -interactable.OriginTransform.localPosition;
        interactable.transform.localRotation = Quaternion.identity;
        interactable.transform.Rotate(adjustedRotation, Space.Self);
        
        trackedInteractable.HideTrackingVolume();
        HighlightDock(false);

        Data = new OccupancyData
        {
            occupied = true,
            gameObject = interactable.gameObject
        };

        isDocked = true;
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

        trackedInteractable.ShowTrackingVolume();
        HighlightDock(true);

        Data = new OccupancyData
        {
            occupied = false,
            gameObject = null
        };

        isDocked = false;
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}