using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

using TMPro;

using static Enum.ControllerEnums;

public class AnalyticsTerminalCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] float refreshInterval = 0.25f;

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
        Debug.Log($"Config.RefreshInterval:{refreshInterval} secs");
        StartCoroutine(MonitorLogs());
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
        HandController.ActuationEventReceived += OnActuation;
        ClearButtonFace.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
        HandController.ActuationEventReceived -= OnActuation;
        ClearButtonFace.EventReceived -= OnEvent;
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

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
    {
        if ((int) characteristics == (int) HandController.LeftHand)
        {
            Debug.Log($"OnActuation Left Hand: {actuation}");
        }
        else if ((int) characteristics == (int) HandController.RightHand)
        {
            Debug.Log($"OnActuation Right Hand : {actuation}");
        }
    }

    public void OnEvent(GameObject gameObject, ButtonFace.EventType type)
    {
        if (gameObject.name.Equals("Clear Button Face") && (type == ButtonFace.EventType.OnEnter))
        {
            Clear();
        }
    }

    private void Clear() => elements.Clear();

    void OnApplicationQuit()
    {
        Debug.Log($"Application terminated after {Time.time} seconds");
    }
}