using UnityEngine;

public abstract class CachedObject<T> : MonoBehaviour
{
    private static CachedObject<T> instance;

    protected virtual void Awake()
    {
        var instances = FindObjectsOfType<CachedObject<T>>();

        if (instances.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = instances[0];
        }
    }

    public static CachedObject<T> Instance { get { return instance; } }
}