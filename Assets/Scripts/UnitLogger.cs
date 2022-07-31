using UnityEngine;

public class UnitLogger : MonoBehaviour
{
    [SerializeField] public bool logEnabled = false;

    public class Prefix
    {
        public string text;
        public Color color = Color.white;
    }

    public void Log(string message, Object context = null) => Log(message, null, context);

    public void Log(string message, Prefix prefix = null, Object context = null)
    {
        #if UNITY_EDITOR
        if (!logEnabled) return;

        string text = (prefix != null) ? $"<color={prefix.color}>{prefix.text}</color> {message}" : message;
        Debug.Log($"{text}", context);
        #endif
    }
}