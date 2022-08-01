using UnityEngine;

public class TextReceiver : MonoBehaviour
{
    public delegate void OnTextEvent(string text);
    public event OnTextEvent EventReceived;

    public void OnText(string text) => EventReceived?.Invoke(text);
}