using UnityEngine;

public class Console : MonoBehaviour
{
    public delegate void LogMessageReceived(string message, float expiration = 0);
    public static event LogMessageReceived logMessageReceived;

    public static void Log(string message, float expiration = 0)
    {
        Debug.Log(message);
        logMessageReceived.Invoke(message, expiration);
    }
}