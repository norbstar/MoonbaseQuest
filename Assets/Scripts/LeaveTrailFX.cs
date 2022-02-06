using System.Collections;

using UnityEngine;

public class LeaveTrailFX : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] GameObject trailPrefab;
    [SerializeField] Vector3 scale = Vector3.one;
    [SerializeField] float interSpawnDelay = 0.1f;

    [Header("Decay")]
    [SerializeField] float duration = 1f;

    private string tempHierarchyName;
    private WaitForSeconds delay;

    protected void Awake()
    {
        tempHierarchyName = $"Temp_{gameObject.GetInstanceID()}";
        delay = new WaitForSeconds(interSpawnDelay);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTrailCoroutine());
    }

    void OnDisable()
    {
        RemoveHierarchy();
    }

    private void RemoveHierarchy()
    {
        var instanceHierarchy = GameObject.Find(tempHierarchyName);

        if (instanceHierarchy != null)
        {
            Destroy(instanceHierarchy);
        }
    }

    private void Decay(GameObject gameObject, float duration)
    {
        StartCoroutine(DecayCoroutine(gameObject, duration));
    }

    private IEnumerator DecayCoroutine(GameObject gameObject, float duration)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>() as Renderer;
        float startTime = Time.unscaledTime;

        while (renderer.material.color.a > 0f)
        {
            float decay = (duration - (Time.unscaledTime - startTime)) / duration;           
            renderer.material.SetColor("_Color", new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, decay));
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator SpawnTrailCoroutine()
    {
        while (isActiveAndEnabled)
        {
            Decay(SpawnPoint(), duration);
            yield return new WaitForSeconds(interSpawnDelay);
        }
    }

    private GameObject SpawnPoint()
    {
        GameObject instance = null;

        if (trailPrefab != null)
        {
            var instanceHierarchy = GameObject.Find(tempHierarchyName);

            if (instanceHierarchy == null)
            {
                instanceHierarchy = new GameObject(tempHierarchyName);
            }

            instance = GameObject.Instantiate(trailPrefab, transform.position, Quaternion.identity) as GameObject;
            instance.transform.SetParent(instanceHierarchy.transform);
            instance.transform.LookAt(transform);
            instance.transform.localScale = scale;
        }

        return instance;
    }
}