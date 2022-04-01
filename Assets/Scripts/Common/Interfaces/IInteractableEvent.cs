using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public interface IInteractableEvent
{
    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force);
}