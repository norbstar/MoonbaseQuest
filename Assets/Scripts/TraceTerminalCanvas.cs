using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class TraceTerminalCanvas : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] int bufferSize = 2000;
    [SerializeField] UnityEvent onReset;

    private GameManager gameManager;

    void Awake()
    {
        ResolveDependencies();
        textUI.text = string.Empty;
    }

    private void ResolveDependencies()
    {
        gameManager = GameManager.GetInstance();
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += Log;
        ClearButtonFace.EventReceived += OnEvent;
        ResetButtonFace.EventReceived += OnEvent;
        gameManager.EventReceived += OnGameManagerEvent;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
        ClearButtonFace.EventReceived -= OnEvent;
        ResetButtonFace.EventReceived -= OnEvent;
        gameManager.EventReceived -= OnGameManagerEvent;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
            case LogType.Warning:
                LogMessage(logString);
                break;

            case LogType.Error:
            case LogType.Exception:
                LogMessageAndStackTrace(logString, stackTrace);
                break;
        }
    }

    private void LogMessage(string logString)
    {
        if (textUI.text.Length > 0)
        {
            textUI.text = $"{textUI.text}\n[{Time.time}] {logString}";
        }
        else
        {
            textUI.text = $"[{Time.time}] {logString}";
        }
    }

    private void LogMessageAndStackTrace(string logString, string stackTrace)
    {
        if (textUI.text.Length > 0)
        {
            textUI.text = $"{textUI.text}\n[{Time.time}] {logString}\n* {stackTrace}";
        }
        else
        {
            textUI.text = $"[{Time.time}] {logString}\n* {stackTrace}";
        }
    }

    public void ScrollToTop()
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    public void ScrollToBottom()
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (textUI.text.Length >= bufferSize)
        {
            textUI.text = textUI.text.Remove(0, textUI.text.Length - bufferSize);
        }

        ScrollToBottom();
    }

    public void OnEvent(GameObject gameObject, ButtonFace.EventType type)
    {
        // Debug.Log($"{this.gameObject.name}.OnEvent:[{gameObject.name}]:Type : {type}");

        if (gameObject.name.Equals("Clear Button Face") && (type == ButtonFace.EventType.OnEnter))
        {
            Clear();
        }
        else if (gameObject.name.Equals("Reset Button Face") && (type == ButtonFace.EventType.OnEnter))
        {
            Reset();
        }
    }

    public void OnGameManagerEvent(GameManager.EventType type, object obj)
    {
        // switch (type)
        // {
        //     case GameManager.EventType.Score:
        //         int score = (int) obj;
        //         Debug.Log($"{gameObject.name}.Score:[{score.ToString()}]");
        //         break;
        // }
    }

    private void Clear() => textUI.text = string.Empty;

    public void Reset() => onReset.Invoke();
}