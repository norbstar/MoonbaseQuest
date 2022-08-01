using UnityEngine.EventSystems;

public interface IPointerEventListener
{
    void OnPointerEvent(PointerEventHandler.Event evt, PointerEventData eventData);
}