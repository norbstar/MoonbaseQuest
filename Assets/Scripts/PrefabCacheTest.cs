using UnityEngine;

[RequireComponent(typeof(PrefabCache))]
public class PrefabCacheTest : MonoBehaviour
{
    [SerializeField] Transform parent;

    private PrefabCache prefabCache;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        // Get an instance from new or from cache
        var instance = prefabCache.GetInstance();
        instance.transform.parent = parent;
    }

    private void ResolveDependencies()
    {
        prefabCache = GetComponent<PrefabCache>() as PrefabCache;
    }
}