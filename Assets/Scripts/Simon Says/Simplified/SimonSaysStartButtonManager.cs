using UnityEngine;

namespace SimonSays.Simplified
{
    public class SimonSaysStartButtonManager : MonoBehaviour
    {
        public delegate void Event(EventType eventType);
        public static event Event EventReceived;
        
        public void OnPressed() => EventReceived?.Invoke(EventType.OnPressed);
        
        public void OnReleased() => EventReceived?.Invoke(EventType.OnReleased);
    }
}