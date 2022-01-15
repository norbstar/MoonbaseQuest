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
    [SerializeField] bool autoStart = true;
    [SerializeField] bool repeat = false;
    [SerializeField] private Color preview;

    protected abstract void SetColor(Color color);

    private Coroutine coroutine;
    private bool inCoroutine = false;

    // Start is called before the first frame update
    void Start()
    {
        if (autoStart)
        {
            coroutine = StartCoroutine(ColorShiftCoroutine());
        }
    }

    public void StartSequence()
    {
        if (!inCoroutine)
        {
            coroutine = StartCoroutine(ColorShiftCoroutine());
        }
    }

    public void StopSequence()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            inCoroutine = false;
        }
    }

    private IEnumerator ColorShiftCoroutine()
    {
        float startTime = Time.time;
        bool complete = false;

        inCoroutine = true;
        
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
            coroutine = StartCoroutine(ColorShiftCoroutine());
        }
    }
}