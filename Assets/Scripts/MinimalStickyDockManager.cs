using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MinimalStickyDockManager : DockManager
{
    public class RigidBodyCache
    {
        public bool useGravity;
    }

    [SerializeField] List<string> supportedTags;

    private new Collider collider;
    private Transform objects;
    private Dictionary<GameObject, RigidBodyCache> properties;

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

    private void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        Debug.Log($"{Time.time} {gameObject.name}.{trigger.name}:OnTriggerEnter");

        var parent = trigger.transform.parent.parent;

        if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            properties.Add(gameObject, new RigidBodyCache
            {
                useGravity = rigidBody.useGravity
            });

            rigidBody.useGravity = false;
            Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
        }

        // if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        // {
        //     rigidBody.useGravity = false;
        //     Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
        // }
    }

    private void OnTriggerExit(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        Debug.Log($"{Time.time} {gameObject.name}.{trigger.name}:OnTriggerExit");

        var parent = trigger.transform.parent.parent;

        if (properties.ContainsKey(gameObject))
        {
            var cache = properties[gameObject];

            if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
            {
                rigidBody.useGravity = cache.useGravity;
                Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
            }
            
            properties.Remove(gameObject);
        }

        // if (parent.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        // {
        //     rigidBody.useGravity = true;
        //     Debug.Log($"{Time.time} {this.gameObject.name}:Use Gravity: {rigidBody.useGravity}");
        // }
    }
}