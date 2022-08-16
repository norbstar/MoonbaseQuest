using System.Collections;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] bool autoStart = false;
    [SerializeField] float initialDelaySec = 2.5f;
    [SerializeField] float fadeInDurationSec = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => canvasGroup = GetComponent<CanvasGroup>() as CanvasGroup;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (autoStart && (canvasGroup != null))
        {
            yield return new WaitForSeconds(initialDelaySec);
            yield return StartCoroutine(FadeInCanvasGroupCoroutine());
        }
    }

    private IEnumerator FadeInCanvasGroupCoroutine()
    {
        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + fadeInDurationSec;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / fadeInDurationSec;
            complete = (fractionComplete >= 1f);
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, fractionComplete);
            yield return null;
        }
    }
}