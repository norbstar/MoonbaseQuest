using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Chess;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewInCodeTest : CustomButtonGroupPanelUIManager
{
    [Header("Config")]
    [SerializeField] float durationSec = 0.5f;

    private ScrollRect scrollRect;

    public override void Awake()
    {
        base.Awake();
        ResolveDependencies();
    }

    private void ResolveDependencies() => scrollRect = GetComponent<ScrollRect>() as ScrollRect;

    public override void OnClickButton(Button button)
    {
        base.OnClickButton(button);

        var name = button.name;

        if (name.Equals("Button A"))
        {
            scrollRect.verticalNormalizedPosition = 1;
        }
        else if (name.Equals("Button B"))
        {
            scrollRect.verticalNormalizedPosition = 0;
        }
        else if (name.Equals("Button C"))
        {
            scrollRect.ScrollToTop();
        }
        else if (name.Equals("Button D"))
        {
            scrollRect.ScrollToBottom();
        }
        else if (name.Equals("Button E") || name.Equals("Player Button") || name.Equals("On Button")) 
        {
            StartCoroutine(ScrollToTopCoroutine());
        }
        else if (name.Equals("Button F") || name.Equals("Bot Button") || name.Equals("Off Button"))
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }
    }

    private IEnumerator ScrollToTopCoroutine()
    {
        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + durationSec;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / durationSec;
            complete = (fractionComplete >= 1f);
            
            scrollRect.verticalNormalizedPosition = fractionComplete;
            
            yield return null;
        }
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        bool complete = false;           
        float startTime = Time.time;            
        float endTime = startTime + durationSec;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / durationSec;
            complete = (fractionComplete >= 1f);
            
            scrollRect.verticalNormalizedPosition = 1f - fractionComplete;
            
            yield return null;
        }
    }
}