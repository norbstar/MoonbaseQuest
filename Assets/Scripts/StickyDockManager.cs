using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickyDockManager : DockManager
{
    // public class RigidBodyCache
    // {
    //     public bool isKinematic;
    //     public bool useGravity;
    // }

    [SerializeField] List<string> supportedTags;

    private new Collider collider;
    private Transform objects;
    // private Dictionary<GameObject, RigidBodyCache> properties;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
        // properties = new Dictionary<GameObject, RigidBodyCache>();
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<Collider>() as Collider;
    }

    void OnEnable()
    {
        InteractableManager.EventReceived += OnEvent;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     Debug.Log($"{this.gameObject.name}.Position:{transform.position}");
    // }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name}:OnTriggerEnter");

        var parent = trigger.transform.parent.parent;
        PrepareObject(trigger, parent);
    }

    private void PrepareObject(GameObject gameObject, Transform parent)
    {
        if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            // properties.Add(gameObject, new RigidBodyCache
            // {
            //     isKinematic = rigidBody.isKinematic,
            //     useGravity = rigidBody.useGravity
            // });

            Debug.Log($"{Time.time} 1");
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        Debug.Log($"{gameObject.name}.{trigger.name}:OnTriggerExit");

        var parent = trigger.transform.parent.parent;
        FreeObject(trigger, parent);
    }

    private void FreeObject(GameObject gameObject, Transform parent)
    {
        // if (properties.ContainsKey(gameObject))
        // {
        //     var cache = properties[gameObject];

        //     if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        //     {
        //         rigidBody.isKinematic = cache.isKinematic;
        //         rigidBody.useGravity = cache.useGravity;
        //     }
            
        //     properties.Remove(gameObject);
        // }

        if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            Debug.Log($"{Time.time} 2");
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
        }

        // if (Object.ReferenceEquals(gameObject.transform.parent, transform))
        // {
        //     gameObject.transform.parent = objects;
        // }
    }

    void OnDisable()
    {
        InteractableManager.EventReceived -= OnEvent;
    }

    public void OnEvent(InteractableManager interactable, InteractableManager.EventType type)
    {
        // Debug.Log($"{this.gameObject.name}.OnEvent.Collider Bounds:{collider.bounds}");
        // Debug.Log($"{this.gameObject.name}.OnEvent.Interactable Position:{interactable.transform.position}");
        
        switch (type)
        {
            case InteractableManager.EventType.OnSelectExited:
                if ((collider.bounds.Contains(interactable.transform.position)) && (supportedTags.Contains(interactable.tag)))
                {
                    // Debug.Log($"{this.gameObject.name}.OnEvent:[{gameObject.name}]:Type : {type}");
                    // interactable.OnExitedInsideStickyDock(gameObject);

                    interactable.transform.parent = transform;
                    interactable.transform.localRotation = Quaternion.identity;
                    interactable.transform.localPosition = Vector3.zero;
                }
                break;
        }
    }
}