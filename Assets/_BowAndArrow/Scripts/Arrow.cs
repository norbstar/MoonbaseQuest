using System.Collections;

using UnityEngine;

public class Arrow : MonoBehaviour
{
    private enum FadeType
    {
        FadeIn,
        FadeOut
    }

    [Header("Renderers")]
    [SerializeField] MeshRenderer shaft;
    [SerializeField] MeshRenderer tip;

    [Header("Config")]
    [SerializeField] float fadeDurationSec = 0.25f;

    private bool isVisible;

    private void SetAlpha(Material material, float alpha) => material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

    private IEnumerator Co_Fade(FadeType fadeType)
    {
        bool complete = false;           
        float startTime = Time.time;            

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / fadeDurationSec;
            complete = (fractionComplete >= 1f);
            float value = default(float);

            switch (fadeType)
            {
                case FadeType.FadeIn:
                    value = Mathf.Lerp(0f, 1f, fractionComplete);
                    break;

                case FadeType.FadeOut:
                    value = Mathf.Lerp(1f, 0f, fractionComplete);
                    break;
            }
            
            SetAlpha(shaft.material, value);
            SetAlpha(tip.material, value);

            yield return null;
        }
    }

    public void Show()
    {
        SetAlpha(shaft.material, 1f);
        SetAlpha(tip.material, 1f);
    }

    public IEnumerator Co_Show()
    {
        SetAlpha(shaft.material, 1f);
        SetAlpha(tip.material, 1f);
        
        yield return Co_Fade(FadeType.FadeIn);
        isVisible = true;
    }

    public void Hide()
    {
        SetAlpha(shaft.material, 0f);
        SetAlpha(tip.material, 0f);
    }

    public IEnumerator Co_Hide()
    {
        yield return Co_Fade(FadeType.FadeOut);

        SetAlpha(shaft.material, 0f);
        SetAlpha(tip.material, 0f);
        isVisible = true;
    }

    public bool IsVisible { get { return isVisible; } }
}