using System.Reflection;

using UnityEngine;

public class SphereManager : MonoBehaviour
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] OnTriggerHandler triggerHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable() => triggerHandler.EventReceived += OnTriggerEvent;
    
    void OnDisable() => triggerHandler.EventReceived -= OnTriggerEvent;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEvent(OnTriggerHandler.EventType type, GameObject gameObject)
    {
        switch (type)
        {
            case OnTriggerHandler.EventType.OnTriggerEnter:
                Debug.Log($"{Time.time} {gameObject.name} {className} OnTriggerEnter");
                break;

            case OnTriggerHandler.EventType.OnTriggerExit:
                Debug.Log($"{Time.time} {gameObject.name} {className} OnTriggerExit");
                break;
        }
    }
}
