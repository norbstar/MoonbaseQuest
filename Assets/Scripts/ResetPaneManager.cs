using UnityEngine;
using UnityEngine.Events;

public class ResetPaneManager : MonoBehaviour
{
    [SerializeField] ResetButton resetButton;
    [SerializeField] UnityEvent onReset;

    void OnEnable()
    {
        ResetButtonFace.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        ResetButtonFace.EventReceived -= OnEvent;
    }

    public void OnEvent(GameObject gameObject, ButtonFace.EventType type)
    {
        // Debug.Log($"{this.gameObject.name}.OnEvent:[{gameObject.name}]:Type : {type}");

        if (gameObject.name.Equals("Reset Button Face") && (type == ButtonFace.EventType.OnEnter))
        {
            Reset();
        }
    }

    public void Reset() => onReset.Invoke();
}