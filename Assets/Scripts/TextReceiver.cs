using UnityEngine;

public abstract class TextReceiver : MonoBehaviour, ITextReceiver
{
    public abstract void OnText(string text);
}