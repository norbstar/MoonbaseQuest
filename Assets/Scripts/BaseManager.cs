using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    protected void Log(string message, Object context = null)
    {
        if (!enableLogging) return;
        Debug.Log(message, context);
    }
}