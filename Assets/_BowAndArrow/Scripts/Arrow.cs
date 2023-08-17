// using System.Collections;

using UnityEngine;

public class Arrow : MonoBehaviour
{
    // private enum FadeType
    // {
    //     FadeIn,
    //     FadeOut
    // }

    [Header("Renderers")]
    [SerializeField] MeshRenderer shaft;
    [SerializeField] MeshRenderer tip;

    // [Header("Config")]
    // [SerializeField] float fadeDurationSec = 0.25f;

    // private bool isVisible;
    
    // public bool IsVisible { get { return isVisible; } }

    private float alpha;

    public float Alpha
    {
        get
        {
            return alpha;
        }

        set
        {
            alpha = value;
            UpdateMaterials();
        }
    }

    private void SetMatrialAlpha(Material material, float alpha) => material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
    
    private void UpdateMaterials()
    {
        SetMatrialAlpha(shaft.material, alpha);
        SetMatrialAlpha(tip.material, alpha);
    }

    // private IEnumerator Co_Fade(FadeType fadeType)
    // {
    //     bool complete = false;           
    //     float startTime = Time.time;            

    //     while (!complete)
    //     {
    //         float fractionComplete = (Time.time - startTime) / fadeDurationSec;
    //         complete = (fractionComplete >= 1f);
    //         float value = default(float);

    //         switch (fadeType)
    //         {
    //             case FadeType.FadeIn:
    //                 value = Mathf.Lerp(0f, 1f, fractionComplete);
    //                 break;

    //             case FadeType.FadeOut:
    //                 value = Mathf.Lerp(1f, 0f, fractionComplete);
    //                 break;
    //         }
            
    //         SetMatrialAlpha(shaft.material, value);
    //         SetMatrialAlpha(tip.material, value);

    //         yield return null;
    //     }
    // }

    public void Show()
    {
        Alpha = 1f;
        SetMatrialAlpha(shaft.material, alpha);
        SetMatrialAlpha(tip.material, alpha);
    }

    // public IEnumerator Co_Show()
    // {
    //     SetMatrialAlpha(shaft.material, 1f);
    //     SetMatrialAlpha(tip.material, 1f);
        
    //     yield return Co_Fade(FadeType.FadeIn);
    //     isVisible = true;
    // }

    public void Hide()
    {
        Alpha = 0f;
        SetMatrialAlpha(shaft.material, alpha);
        SetMatrialAlpha(tip.material, alpha);
    }

    // public IEnumerator Co_Hide()
    // {
    //     yield return Co_Fade(FadeType.FadeOut);

    //     SetMatrialAlpha(shaft.material, 0f);
    //     SetMatrialAlpha(tip.material, 0f);
    //     isVisible = true;
    // }
}