using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickyDockManager : DockManager
{
    public class RigidBodyCache
    {
        public bool useGravity;
    }

    [SerializeField] List<string> supportedTags;

    private new Collider collider;
    private Transform objects;
    private Dictionary<GameObject, RigidBodyCache> properties;
    // private Rigidbody cachedProperties;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        properties = new Dictionary<GameObject, RigidBodyCache>();
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<Collider>() as Collider;
    }

    void OnEnable()
    {
        InteractableManager.EventReceived += OnEvent;
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        // Debug.Log($"{Time.time} {gameObject.name}.{trigger.name}:OnTriggerEnter");
        Debug.Log($"{Time.time} {gameObject.name} 1");

        var parent = trigger.transform.parent.parent;
        PrepareObject(trigger, parent);
    }

    private void PrepareObject(GameObject gameObject, Transform parent)
    {
        Debug.Log($"{Time.time} {gameObject.name} 2");
        if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            Debug.Log($"{Time.time} {gameObject.name} 3");
            properties.Add(gameObject, new RigidBodyCache
            {
                useGravity = rigidBody.useGravity
            });

            // cachedProperties = new Rigidbody
            // {
            //     useGravity = rigidBody.useGravity
            // };

            rigidBody.useGravity = false;
            // Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        // Debug.Log($"{Time.time} {gameObject.name}.{trigger.name}:OnTriggerExit");
        Debug.Log($"{Time.time} {gameObject.name} 4");

        var parent = trigger.transform.parent.parent;
        FreeObject(trigger, parent);
    }

    private void FreeObject(GameObject gameObject, Transform parent)
    {
        Debug.Log($"{Time.time} {gameObject.name} 5");
        if (properties.ContainsKey(gameObject))
        {
            Debug.Log($"{Time.time} {gameObject.name} 6");
            var cache = properties[gameObject];

            if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
            {
                rigidBody.useGravity = cache.useGravity;
                // Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
                Debug.Log($"{Time.time} {gameObject.name} 7");
            }
            
            properties.Remove(gameObject);
        }

        // if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        // {
        //     rigidBody.useGravity = cachedProperties.useGravity;
        // }

        if (Object.ReferenceEquals(gameObject.transform.parent, transform))
        {
            Debug.Log($"{Time.time} {gameObject.name} 8");
            gameObject.transform.parent = objects;
        }
    }

    void OnDisable()
    {
        InteractableManager.EventReceived -= OnEvent;
    }

    public void OnEvent(InteractableManager interactable, InteractableManager.EventType type)
    {
        // Debug.Log($"{Time.time} {gameObject.name}.OnEvent.Collider Bounds:{collider.bounds}");
        // Debug.Log($"{Time.time} {gameObject.name}.OnEvent.Interactable Position:{interactable.transform.position}");
        // Debug.Log($"{Time.time} {gameObject.name}.OnEvent:Interactable : {interactable.name} Type : {type}");
        Debug.Log($"{Time.time} {gameObject.name} 9");

        switch (type)
        {
            case InteractableManager.EventType.OnSelectEntered:
                Debug.Log($"{Time.time} {gameObject.name} 10");
                if (Object.ReferenceEquals(Data.gameObject, interactable.gameObject))
                {
                    Debug.Log($"{Time.time} {gameObject.name} 11");
                    UndockInteractable();
                }

                break;

            case InteractableManager.EventType.OnSelectExited:
                Debug.Log($"{Time.time} {gameObject.name} 12");
                if (Data.occupied) return;

                if ((collider.bounds.Contains(interactable.transform.position)) && (supportedTags.Contains(interactable.tag)))
                {
                    Debug.Log($"{Time.time} {gameObject.name} 13");
                    DockInteractable(interactable);
                }
                break;
        }
    }

    private void DockInteractable(InteractableManager interactable)
    {
        Debug.Log($"{Time.time} {gameObject.name} 14");
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
        Debug.Log($"{Time.time} {gameObject.name} 15");
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            Debug.Log($"{Time.time} {gameObject.name} 16");
            rigidBody.isKinematic = true;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    private void UndockInteractable()
    {
        Debug.Log($"{Time.time} {gameObject.name} 17");
        Data = new OccupancyData
        {
            occupied = false,
            gameObject = null
        };
    }
}