using UnityEngine;

public class AnimationCompleteEvent : MonoBehaviour
{
    public delegate void OnAnimationComplete();

    public OnAnimationComplete OnComplete;

    public void OnEvent()
    {
        OnComplete?.Invoke();
    }
}