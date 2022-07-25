using System.Collections;

using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FadeOut : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float fadeInDurationSec = 0.5f;
    [SerializeField] bool destroyOnCompletion = true;

    private new MeshRenderer renderer;

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => renderer = GetComponent<MeshRenderer>() as MeshRenderer;

    // Start is called before the first frame update
    void Start() => StartCoroutine(FadeOutCoroutine());

    private IEnumerator FadeOutCoroutine()
    {
        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + fadeInDurationSec;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / fadeInDurationSec;
            complete = (fractionComplete >= 1f);
            
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f - fractionComplete);
            
            yield return null;
        }

        if (destroyOnCompletion) Destroy(gameObject);
    }
}