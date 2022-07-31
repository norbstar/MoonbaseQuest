using System.Collections;

using UnityEngine;

[RequireComponent(typeof(FX.ScaleFX))]
public class ScaleFXManager : MonoBehaviour
{
    private FX.ScaleFX scaleFX;
    private Coroutine coroutine;

    public IEnumerator ScaleUp(float fromValue, float toValue)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (scaleFX == null)
        {
            scaleFX = GetComponent<FX.ScaleFX>() as FX.ScaleFX;
        }

        yield return coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Config
        {
            fromValue = fromValue,
            toValue = toValue
        }));
    }

    public IEnumerator ScaleDown(float fromValue, float toValue)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (scaleFX == null)
        {
            scaleFX = GetComponent<FX.ScaleFX>() as FX.ScaleFX;
        }

        yield return coroutine = StartCoroutine(scaleFX.Apply(new FX.ScaleFX.Config
        {
            fromValue = fromValue,
            toValue = toValue
        }));
    }
}