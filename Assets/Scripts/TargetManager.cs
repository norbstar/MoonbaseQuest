using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class TargetManager : MonoBehaviour, IInteractableEvent
{
    [SerializeField] AudioClip hitClip;

    public void OnActivate(XRGrabInteractable interactable)
    {
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);
        Activate(interactable);
    }

    public abstract void Activate(XRGrabInteractable interactable);
}