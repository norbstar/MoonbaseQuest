using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using TMPro;

public class ConsoleTest : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;

    public class Payload
    {
        public string value;
        public bool expires;
        public float expiration;
    }

    private GameObject asteroidSpawner;
    private IDictionary<string, Payload> elements = new Dictionary<string, Payload>();
    private float lastTime;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        asteroidSpawner = GameObject.Find("Asteroid Spawner");
    }

    void OnEnable()
    {
        Console.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Console.logMessageReceived -= Log;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MonitorLogs());
        lastTime = Time.time;

        // Console.Log($"Test message");
        // StartCoroutine(PostContent());
    }

    // private IEnumerator PostContent()
    // {
    //     for (int idx = 0; idx < 50; idx++)
    //     {
    //         PostConsoleLog("Asteroid Count", (asteroidSpawner.transform.childCount - 1).ToString());
    //         yield return new WaitForSeconds(0.25f);
    //     }
    // }

    public void Log(string message, float expiration = 0)
    {
        // Debug.Log($"{gameObject.name}.Log:Message {message} Expiration: {expiration}");
        LogMessage(message, expiration);
    }

    private void LogMessage(string message, float expiration = 0)
    {
        string[] splitString = message.Split(new[] { ':' }, 2);
        string key = $"{Time.time} {splitString[0]}";
        string value = (splitString.Length > 1) ? splitString[1] : null;

        // if (value != null)
        // {
        //     Debug.Log($"LogMessage Key : {key} Value : {value}");
        // }
        // else
        // {
        //     Debug.Log($"LogMessage Key : {key}");
        // }

        if (elements.ContainsKey(key))
        {
            elements[key] = new Payload
            {
                value = value,
                expires = (expiration != 0),
                expiration = Time.time + expiration
            };
        }
        else
        {
            elements.Add(key, new Payload
            {
                value = value,
                expires = (expiration != 0),
                expiration = Time.time + expiration
            });
        }
    }

    private IEnumerator MonitorLogs()
    {
        while (isActiveAndEnabled)
        {
            textUI.text = string.Empty;

            var sortedElements = from e in elements orderby e.Key select e;

            for (int idx = 0; idx < sortedElements.Count(); idx++)
            {
                KeyValuePair<string, Payload> element = sortedElements.ElementAt(idx);
                
                var expires = element.Value.expires;
                var expiration = element.Value.expiration;
                
                if ((!expires) || ((expires) && (Time.time <= expiration)))
                {
                    if (textUI.text.Length > 0)
                    {
                        textUI.text = $"{textUI.text}\n";
                    }

                    textUI.text = (element.Value.value != null) ? $"{textUI.text}{element.Key} : {element.Value.value}" : $"{textUI.text}{element.Key}";
                }
                else
                {
                    elements.Remove(element);
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    // private void PostConsoleLog(string key, string value)
    // {
    //     // Debug.Log($"{key} : {value}");
    //     Console.Log($"{Time.time} {key}:{value}", 5f);
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     // if (Time.time > lastTime)
    //     // {
    //     //     Console.Log($"{Time.time} Asteroid Count:{asteroidSpawner.transform.childCount - 1}, 1f");
    //     //     lastTime = Time.time + 1f;
    //     // }

    //     Debug.Log($"Element Count : {elements.Count}");
    // }
}