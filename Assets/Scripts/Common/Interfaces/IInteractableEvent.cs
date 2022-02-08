using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public interface IInteractableEvent
{
    public void OnActivate(XRGrabInteractable interactable, Vector3 origin, Vector3 hitPoint);
}