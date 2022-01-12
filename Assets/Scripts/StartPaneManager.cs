using UnityEngine;
using UnityEngine.Events;

public class StartPaneManager : MonoBehaviour
{
    [SerializeField] StartButton startButton;
    [SerializeField] UnityEvent onStartGame;

    void OnEnable()
    {
        startButton.EventReceived += EventReceived;
    }

    void OnDisable()
    {
        startButton.EventReceived -= EventReceived;
    }

    public void EventReceived(StartButton.Event evt)
    {
        // Debug.Log($"{Time.time} {gameObject.name}.EventReceived:Event : {evt}");

        if (evt == StartButton.Event.OnTrigger)
        {
            Enable(false);
            onStartGame?.Invoke();
        }
    }

    public void Enable(bool enable = true)
    {
        gameObject.SetActive(enable);
    }
}