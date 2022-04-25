using UnityEngine;

namespace Chess.Button
{
    public class ButtonEventManager : MonoBehaviour
    {
        [SerializeField] protected ButtonId id;
        
        public enum ButtonId
        {
            LowerTable,
            RaiseTable,
            ResetTable,
            ResetBoard,
            AudioOn,
            AudioOff,
            BotOn,
            BotOff,
            LaunchScene
        }

        public delegate void Event(ButtonEventManager manager, ButtonId id, ButtonEventType eventType);
        public static event Event EventReceived;
        
        public void OnPressed() => EventReceived?.Invoke(this, id, ButtonEventType.OnPressed);

        public void OnReleased() => EventReceived?.Invoke(this, id, ButtonEventType.OnReleased);
    }
}