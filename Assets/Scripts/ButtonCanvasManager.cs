using UnityEngine;

public class ButtonCanvasManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    public void OnExit()
    {
        Log($"{gameObject.name}.OnExit");
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}