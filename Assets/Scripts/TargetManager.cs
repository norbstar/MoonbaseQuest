using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class TargetManager : MonoBehaviour, IInteractableEvent
{
    [SerializeField] AudioClip hitClip;

    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force)
    {
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);
        Activate(interactable, hitPoint);
    }

    public abstract void Activate(XRGrabInteractable interactable, Vector3 hitPoint);
}