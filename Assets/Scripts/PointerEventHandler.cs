using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Event
    {
        Enter,
        Exit
    }

    public delegate void OnPointerEvent(Event evt, PointerEventData pointerEventData);
    public event OnPointerEvent EventReceived;

    public void OnPointerEnter(PointerEventData pointerEventData) => EventReceived?.Invoke(Event.Enter, pointerEventData);

    public void OnPointerExit(PointerEventData pointerEventData) => EventReceived?.Invoke(Event.Exit, pointerEventData);
}