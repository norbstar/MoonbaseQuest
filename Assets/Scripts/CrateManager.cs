using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CrateManager : BaseManager, IInteractableEvent
{
    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force)
    {
        Log($"{gameObject.name}.OnActivate:{interactable.name} Hit Point : {hitPoint} Force : {force}");
    }
}