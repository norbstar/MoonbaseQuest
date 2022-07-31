using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    protected void Log(string message, string prefix = default(string), Object context = null)
    {
        #if UNITY_EDITOR
            if (!enableLogging) return;
            Debug.Log($"<color=green>{prefix}</color> {message}", context);
        #endif
    }
}