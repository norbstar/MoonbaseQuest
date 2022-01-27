using UnityEngine.XR.Interaction.Toolkit;

public interface IInteractable
{
    void OnSelectEntered(SelectEnterEventArgs args);
    void OnSelectExited(SelectExitEventArgs args);
    void OnDockStatusChange(bool isDocked);
}