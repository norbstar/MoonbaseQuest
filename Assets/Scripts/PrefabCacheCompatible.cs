using UnityEngine;

public class PrefabCacheCompatible : MonoBehaviour, ICache
{
    private PrefabCache provider;

    public PrefabCache Provider { set { provider = value; } }

    public void Destroy()
    {
        provider.Cache(gameObject);
    }
}