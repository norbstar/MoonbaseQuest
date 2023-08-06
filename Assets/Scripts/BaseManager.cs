using UnityEngine;

// The BaseManager class is a contextual class designed to provide conditional logging support.
// To enable the feature the enableLogging flag needs to be set in the inspector.
// Unless the confineLoggingToEditor flag is also set, logging is confined to the editor.
public class BaseManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool enableLogging = false;
    [SerializeField] bool confineLoggingToEditor = false;

    protected void Log(string message, string prefix = default(string), Object context = null)
    {
        if (!enableLogging) return;

        bool log = true;

        if (confineLoggingToEditor)
        {
            #if !UNITY_EDITOR
                log = false;
            #endif
        }

        if (!log) return;

        if (prefix == null || prefix.Length > 0)
        {
            Debug.Log($"<color=green>{prefix}</color> {message}", context);
        }
        else
        {
            Debug.Log($"{message}", context);
        }
    }
}