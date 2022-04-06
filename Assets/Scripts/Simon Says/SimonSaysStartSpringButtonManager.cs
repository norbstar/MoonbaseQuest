using UnityEngine;

namespace SimonSays
{
    public class SimonSaysStartSpringButtonManager : MonoBehaviour
    {
        public delegate void Event(EventType eventType);
        public static event Event EventReceived;
        
        public void OnPressed() => EventReceived?.Invoke(EventType.OnPressed);

        public void OnReleased() => EventReceived?.Invoke(EventType.OnReleased);
    }
}