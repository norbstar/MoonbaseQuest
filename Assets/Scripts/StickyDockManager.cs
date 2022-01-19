using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickyDockManager : DockManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Restrictions")]
    [SerializeField] List<string> supportedTags;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private new Collider collider;
    private Transform objects;

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

    // private void OnTriggerEnter(Collider collider)
    // {
    //     GameObject trigger = collider.gameObject;
    //     var parent = trigger.transform.parent.parent;
    //     PrepareObject(trigger, parent);
    // }

    // private void PrepareObject(GameObject gameObject, Transform parent)
    // {
    // }

    // private void OnTriggerExit(Collider collider)
    // {
    //     GameObject trigger = collider.gameObject;
    //     var parent = trigger.transform.parent.parent;
    //     FreeObject(trigger, parent);
    // }

    // private void FreeObject(GameObject gameObject, Transform parent)
    // {
    //     if (Object.ReferenceEquals(gameObject.transform.parent, transform))
    //     {
    //         gameObject.transform.parent = objects;
    //     }
    // }

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

                Log($"{Time.time} {gameObject.name} {className} OnEvent.OnSelectEntered.Is Not Occupied");
                if ((collider.bounds.Contains(interactable.transform.position)) && (supportedTags.Contains(interactable.tag)))
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

        interactable.transform.parent = transform;
        interactable.transform.localRotation = Quaternion.identity;
        interactable.transform.localPosition = Vector3.zero;

        Data = new OccupancyData
        {
            occupied = true,
            gameObject = interactable.gameObject
        };
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