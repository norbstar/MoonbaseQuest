using System.Collections;

using UnityEngine;

namespace SimonSays.Simplified
{
    public class SimonSaysButtonManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] Id id;
        public Id ButtonId { get { return id; } }

        [SerializeField] SimpleHandButtonManager buttonManager;

        public delegate void Event(Id id, EventType eventType);
        public static event Event EventReceived;

        public IEnumerator SimulateButtonPressCoroutine(float timeframe)
        {
            buttonManager.OnSimulateOnPressed();
            yield return new WaitForSeconds(timeframe);
            buttonManager.OnSimulateOnReleased();
        }

        public void OnPressed() => EventReceived?.Invoke(id, EventType.OnPressed);

        public void OnReleased() => EventReceived?.Invoke(id, EventType.OnReleased);
    }
}