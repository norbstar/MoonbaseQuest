using UnityEngine;

public abstract class CachedObject<T> : MonoBehaviour
{
    protected virtual void Awake()
    {
        int instances = FindObjectsOfType<CachedObject<T>>().Length;

        if (instances > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}