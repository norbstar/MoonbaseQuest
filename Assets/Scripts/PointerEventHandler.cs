using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Event
    {
        Enter,
        Exit
    }

    public delegate void OnPointerEvent(GameObject gameObject, Event evt, PointerEventData pointerEventData);
    public event OnPointerEvent EventReceived;

    public void OnPointerEnter(PointerEventData pointerEventData) => EventReceived?.Invoke(gameObject, Event.Enter, pointerEventData);

    public void OnPointerExit(PointerEventData pointerEventData) => EventReceived?.Invoke(gameObject, Event.Exit, pointerEventData);
}