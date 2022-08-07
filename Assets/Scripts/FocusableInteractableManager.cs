using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FocusableInteractableManager : InteractableManager, IFocus
{
    public bool HasFocus { get { return hasFocusCount > 0; } }
    public bool HasMultiFocus { get { return hasFocusCount > 1; } }

    private int hasFocusCount;

    public void GainedFocus(GameObject focusObject, Vector3 focalPoint)
    {
        if (!HasFocus)
        {
            OnFocusGained();
        }

        ++hasFocusCount;
    }

    protected virtual void OnFocusGained() { }

    public void LostFocus(GameObject focusObject)
    {
        if (!HasMultiFocus)
        {
            OnFocusLost();
        }

        --hasFocusCount;
    }

    protected virtual void OnFocusLost() { }
}