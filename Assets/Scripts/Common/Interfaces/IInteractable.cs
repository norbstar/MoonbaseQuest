using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

public interface IInteractable
{
    GameObject GetGameObject();
    void OnSelectEntered(SelectEnterEventArgs args);
    void OnSelectExited(SelectExitEventArgs args);
    void OnDockStatusChange(bool isDocked);
    void OnOpposingEvent(State state, bool isTrue, IInteractable obj);
}