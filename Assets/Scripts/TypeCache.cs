using System;
using System.Collections.Generic;

using UnityEngine;

public abstract class TypeCache<T> : MonoBehaviour
{
    [SerializeField] private List<T> cache;

    public List<T> Cache { get { return cache; } }

    // Start is called before the first frame update
    void Start()
    {
        cache = new List<T>();
    }

//     T GetInstance<T>() where T : new()
// {
//     T instance = new T();
//     return instance;
// }

    // public virtual T GetInstance<T>() where T : new()
    public virtual T GetInstance()
    {
        T instance = default(T);

        if (cache.Count > 0)
        {
            instance = cache[0];
            cache.Remove(instance);
        }
        else
        {
            // instance = new T();
            Type type = typeof(T);
            instance = (T) Activator.CreateInstance(type);
        }
        
        return instance;
    }
}