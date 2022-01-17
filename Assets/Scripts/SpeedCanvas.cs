using UnityEngine;
using UnityEngine.Events;

public class SpeedCanvas : MonoBehaviour
{
    [SerializeField] UnityEvent onSetLowSpeed, onSetMediumSpeed, onSetHighSpeed;

    void OnEnable()
    {
        SpeedButtonFace.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        SpeedButtonFace.EventReceived -= OnEvent;
    }

    public void OnEvent(GameObject gameObject, ButtonFace.EventType type)
    {
        // Debug.Log($"{this.gameObject.name}.OnEvent:[{gameObject.name}]:Type : {type}");

        if (type == ButtonFace.EventType.OnEnter)
        {
            if (gameObject.name.Equals("Low Speed Button Face"))
            {
                SetLowSpeed();
            }
            else if (gameObject.name.Equals("Medium Speed Button Face"))
            {
                SetMediumSpeed();
            }
            else if (gameObject.name.Equals("High Speed Button Face"))
            {
                SetHighSpeed();
            }
        }
    }

    public void SetLowSpeed() => onSetLowSpeed.Invoke();

    public void SetMediumSpeed() => onSetMediumSpeed.Invoke();

    public void SetHighSpeed() => onSetHighSpeed.Invoke();
}