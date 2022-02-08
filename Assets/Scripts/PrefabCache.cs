using System.Collections.Generic;

using UnityEngine;

public class PrefabCache : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    [SerializeField] private List<GameObject> cache;

    public GameObject GetInstance()
    {
        GameObject instance = default(GameObject);

        if (transform.childCount > 0)
        {
            instance = transform.GetChild(0).gameObject;
            cache.Remove(instance);
        }
        else
        {
            var prefabInstance = GameObject.Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            
            if (prefabInstance.TryGetComponent<PrefabCacheCompatible>(out PrefabCacheCompatible prefabCacheCompatible))
            {
                prefabCacheCompatible.Provider = this;
                instance = prefabInstance;
            }
        }
        
        return instance;
    }

    public void Cache(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<PrefabCacheCompatible>(out PrefabCacheCompatible prefabCacheCompatible))
        {
            gameObject.transform.parent = transform;
            this.cache.Add(gameObject);
        }
    }
}