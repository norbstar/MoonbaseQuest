using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class TargetManager : MonoBehaviour, IInteractableEvent
{
    [SerializeField] AudioClip hitClip;

    public void OnActivate(XRGrabInteractable interactable, Vector3 hitPoint)
    {
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);
        Activate(interactable, hitPoint);
    }

    public abstract void Activate(XRGrabInteractable interactable, Vector3 hitPoint);
}