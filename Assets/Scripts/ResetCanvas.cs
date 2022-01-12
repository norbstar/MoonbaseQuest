using UnityEngine;
using UnityEngine.Events;

public class ResetCanvas : MonoBehaviour
{
    [SerializeField] UnityEvent onReset;

    void OnEnable()
    {
        ResetButtonFace.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        ResetButtonFace.EventReceived -= OnEvent;
    }

    public void OnEvent(GameObject gameObject, ButtonFace.State state)
    {
        // Debug.Log($"{gameObject.name}.OnEvent:[{gameObject.name}]");

        if (gameObject.name.Equals("Reset Button Face") && (state == ButtonFace.State.OnEnter))
        {
            Reset();
        }
    }

    public void Reset() => onReset.Invoke();
}