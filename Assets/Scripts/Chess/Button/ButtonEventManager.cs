using UnityEngine;

namespace Chess.Button
{
    public class ButtonEventManager : MonoBehaviour
    {
        [SerializeField] Id id;
        
        public enum Id
        {
            Reset,
            LowerTable,
            RaiseTable,
            LaunchScene
        }

        public delegate void Event(Id id, ButtonEventType eventType);
        public static event Event EventReceived;
        
        public void OnPressed() => EventReceived?.Invoke(id, ButtonEventType.OnPressed);

        public void OnReleased() => EventReceived?.Invoke(id, ButtonEventType.OnReleased);
    }
}