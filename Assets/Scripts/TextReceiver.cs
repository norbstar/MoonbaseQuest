using UnityEngine;

public class TextReceiver : MonoBehaviour, ITextReceiver
{
    public virtual void OnText(string text) { }
}