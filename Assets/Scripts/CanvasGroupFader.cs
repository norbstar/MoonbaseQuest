using System.Collections;

using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float initialDelaySec = 2.5f;
    [SerializeField] float fadeInDurationSec = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => canvasGroup = GetComponent<CanvasGroup>() as CanvasGroup;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeInCanvasGroupCoroutine());
        }
    }

    private IEnumerator FadeInCanvasGroupCoroutine()
    {
        yield return new WaitForSeconds(initialDelaySec);

        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + fadeInDurationSec;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / fadeInDurationSec;
            complete = (fractionComplete >= 1f);
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, fractionComplete);

            // float value = GetValueInRange(startTime, fadeInDurationSec, 0f, 1f);
            // complete = (value >= 1f);
            // canvasGroup.alpha = value;
            
            yield return null;
        }
    }

    // private float GetValueInRange(float startTime, float duration, float minValue, float maxValue)
    // {
    //     float endTime = startTime + duration;
    //     float fractionComplete = (Time.time - startTime) / duration;
    //     bool complete = (fractionComplete >= maxValue);

    //     return Mathf.Lerp(minValue, maxValue, fractionComplete);
    // }
}