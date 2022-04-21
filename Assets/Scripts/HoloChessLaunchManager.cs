using UnityEngine;

using Chess.Button;

public class HoloChessLaunchManager : MonoBehaviour
{
    void OnEnable()
    {
        ButtonEventManager.EventReceived += OnButtonEvent;
    }

    void OnDisable()
    {
        ButtonEventManager.EventReceived -= OnButtonEvent;
    }

    private void OnButtonEvent(ButtonEventManager manager, ButtonEventManager.ButtonId id, ButtonEventType eventType)
    {
        if (eventType == ButtonEventType.OnPressed)
        {
            switch (id)
            {
                case ButtonEventManager.ButtonId.LaunchScene:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Chess");
                    break;
            }
        }
        else if (eventType == ButtonEventType.OnReleased)
        {
            switch (id)
            {
                case ButtonEventManager.ButtonId.LaunchScene:
                    break;
            }
        }
    }
}