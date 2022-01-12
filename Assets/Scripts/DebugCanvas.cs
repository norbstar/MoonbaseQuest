using System;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class DebugCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;

    private IDictionary<string, string> elements = new Dictionary<string, string>();
    private int boo;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
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
                break;

            default:
                elements.Add("!!!", logString);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ++boo;
        Debug.Log($"Clock:{DateTime.Now.ToString()}");
        Debug.Log($"Frames:{boo.ToString()}");

        textUI.text = string.Empty;

        foreach(KeyValuePair<string, string> element in elements)
        {
            if (textUI.text.Length > 0)
            {
                textUI.text = $"{textUI.text}\n";
            }

            textUI.text = (element.Value != null) ? $"{textUI.text}{element.Key} : {element.Value}" : $"{textUI.text}{element.Key}";
        }
    }
}