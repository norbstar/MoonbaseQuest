using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Event
    {
        Enter,
        Down,
        Up,
        Exit
    }

    public delegate void OnPointerEvent(GameObject gameObject, Event evt, PointerEventData pointerEventData);
    public event OnPointerEvent EventReceived;

    public void OnPointerEnter(PointerEventData eventData) => EventReceived?.Invoke(gameObject, Event.Enter, eventData);

    public void OnPointerDown(PointerEventData eventData) => EventReceived?.Invoke(gameObject, Event.Down, eventData);

    public void OnPointerUp(PointerEventData eventData) => EventReceived?.Invoke(gameObject, Event.Up, eventData);

    public void OnPointerExit(PointerEventData eventData) => EventReceived?.Invoke(gameObject, Event.Exit, eventData);
}