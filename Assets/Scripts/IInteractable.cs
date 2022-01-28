using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public interface IInteractable
{
    GameObject GetGameObject();
    void OnSelectEntered(SelectEnterEventArgs args);
    void OnSelectExited(SelectExitEventArgs args);
    void OnDockStatusChange(bool isDocked);
    void OnOpposingEvent(HandController.State state, bool isTrue, IInteractable obj);
}