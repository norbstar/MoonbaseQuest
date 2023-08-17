using UnityEngine;

public class ProximityHandlerTest : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Arrow arrow;

    [Header("Config")]
    [SerializeField] OnProximityHandler proximityHandler;
    [SerializeField] GameObject target;

    void Awake() => arrow.Alpha = 0f;

    void OnEnable() => proximityHandler.EventReceived += OnProximityHandler;

    void OnDisable() => proximityHandler.EventReceived -= OnProximityHandler;

    private void OnProximityHandler(float distance, GameObject gameObject)
    {
        Debug.Log($"OnProximityHandler:Status Radius: {proximityHandler.Radius}");

        if (!GameObject.ReferenceEquals(gameObject, target)) return;

        float alpha = 1f - (distance / proximityHandler.Radius);
        arrow.Alpha = alpha;

        Debug.Log($"OnProximityHandler:Calcs GameObject: {gameObject.name} Distance: {distance} Alpha: {alpha}");
    }
}
