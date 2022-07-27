using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeOutImage : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float fadeInDurationSec = 0.5f;
    [SerializeField] bool destroyOnCompletion = true;

    private Image image;
    private float fractionComplete;
    public float FractionComplete { get { return fractionComplete; } }

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => image = GetComponent<Image>() as Image;

    public void Go() => StartCoroutine(FadeOutCoroutine());

    private IEnumerator FadeOutCoroutine()
    {
        Debug.Log("FadeOutCoroutine");

        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + fadeInDurationSec;

        while (!complete)
        {
            fractionComplete = (Time.time - startTime) / fadeInDurationSec;
            complete = (fractionComplete >= 1f);
            
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f - fractionComplete);
            
            yield return null;
        }

        if (destroyOnCompletion) Destroy(gameObject);
    }
}