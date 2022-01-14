using System.Collections;

using UnityEngine;

public abstract class ColorSequencer : MonoBehaviour
{
    public enum Type
    {
        LeftToRight,
        RightToLeft
    }

    [SerializeField] Gradient gradient;
    [SerializeField] float duration = 10.0f;
    [SerializeField] Type type;
    [SerializeField] bool repeat = false;
    [SerializeField] private Color preview;

    protected abstract void SetColor(Color color);

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(ColorShiftCoroutine());
    }

    private IEnumerator ColorShiftCoroutine()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            if (type == Type.LeftToRight)
            {
                var color = gradient.Evaluate(fractionComplete);
                SetColor(color);
                preview = color;
            }
            else if (type == Type.RightToLeft)
            {
                var color = gradient.Evaluate(1f - fractionComplete);
                SetColor(color);
                preview = color;
            }

            complete = (fractionComplete >= 1f);

            if (complete)
            {
                OnComplete();
            }

            yield return null;
        }
    }

    private void OnComplete()
    {
        if (repeat)
        {
            StartCoroutine(ColorShiftCoroutine());
        }
    }
}