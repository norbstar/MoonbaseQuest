using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
// using UnityEngine.Events;

using TMPro;

public class AnalyticsTerminalCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] float refreshInterval = 0.5f;

    private IDictionary<string, string> elements = new Dictionary<string, string>();
    private bool refresh;
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

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log($"{gameObject.name}.RefreshInterval:{refreshInterval} secs");
        StartCoroutine(MonitorLogs());
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
        refresh = true;

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
        string[] splitString = logString.Split(new[] { ':' }, 2);
        string key = splitString[0];
        string value = (splitString.Length > 1) ? splitString[1] : null;

        if (elements.ContainsKey(key))
        {
            elements[key] = value;
        }
        else
        {
            elements.Add(key, value);
        }
    }

    private void LogMessageAndStackTrace(string logString, string stackTrace)
    {
        string[] splitString = logString.Split(new[] { ':' }, 2);
        string key = splitString[0];
        string value = (splitString.Length > 1) ? $"{splitString[1]} {stackTrace}" : $"{stackTrace}";
        
        if (elements.ContainsKey(key))
        {
            elements[key] = value;
        }
        else
        {
            elements.Add(key, value);
        }
    }

    private IEnumerator MonitorLogs()
    {
        while (isActiveAndEnabled)
        {
            if (refresh)
            {
                refresh = false;
                textUI.text = string.Empty;

                var sortedElements = from e in elements orderby e.Key select e;

                foreach(KeyValuePair<string, string> element in sortedElements)
                {
                    if (textUI.text.Length > 0)
                    {
                        textUI.text = $"{textUI.text}\n";
                    }

                    textUI.text = (element.Value != null) ? $"{textUI.text}{element.Key} : {element.Value}" : $"{textUI.text}{element.Key}";
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (refresh)
    //     {
    //         refresh = false;
    //         textUI.text = string.Empty;

    //         var sortedElements = from e in elements orderby e.Key select e;

    //         foreach(KeyValuePair<string, string> element in sortedElements)
    //         {
    //             if (textUI.text.Length > 0)
    //             {
    //                 textUI.text = $"{textUI.text}\n";
    //             }

    //             textUI.text = (element.Value != null) ? $"{textUI.text}{element.Key} : {element.Value}" : $"{textUI.text}{element.Key}";
    //         }
    //     }
    // }

    public void OnEvent(GameObject gameObject, ButtonFace.EventType type)
    {
        // Debug.Log($"{this.gameObject.name}.OnEvent:[{gameObject.name}]:Type : {type}");

        if (gameObject.name.Equals("Clear Button Face") && (type == ButtonFace.EventType.OnEnter))
        {
            Clear();
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

    private void Clear() => elements.Clear();
}