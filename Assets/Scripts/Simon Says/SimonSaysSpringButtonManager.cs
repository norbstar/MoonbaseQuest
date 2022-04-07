using System.Collections;

using UnityEngine;

namespace SimonSays
{
    public class SimonSaysSpringButtonManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] Id id;
        public Id ButtonId { get { return id; } }

        [Header("Clicker")]
        [SerializeField] Renderer clickerRenderer;

        [Header("Materials")]
        [SerializeField] Material defaultMaterial;
        [SerializeField] Material highlightMaterial;

        public delegate void Event(Id id, EventType eventType);
        public static event Event EventReceived;

        // Start is called before the first frame update
        void Start()
        {
            clickerRenderer.material = defaultMaterial;
        }

        public IEnumerator SimulateButtonPressCoroutine(float timeframe)
        {
            OnPressed();
            yield return new WaitForSeconds(timeframe);
            OnReleased();
        }

        public void OnPressed()
        {
            clickerRenderer.material = highlightMaterial;
            EventReceived?.Invoke(id, EventType.OnPressed);
        }

        public void OnReleased()
        {
            clickerRenderer.material = defaultMaterial;
            EventReceived?.Invoke(id, EventType.OnReleased);
        }
    }
}