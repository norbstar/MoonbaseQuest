using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FocusableInteractableManager : InteractableManager, IFocus
{
    [Header("Identity")]
    [SerializeField] IdentityCanvasManager identityCanvasManager;

    public bool HasFocus { get { return hasFocusCount > 0; } }
    public bool HasMultiFocus { get { return hasFocusCount > 1; } }

    private int hasFocusCount;

    protected override void Awake()
    {
        base.Awake();
        identityCanvasManager.IdentityText = gameObject.name;
    }

    public void GainedFocus(GameObject focusObject)
    {
        identityCanvasManager.gameObject.SetActive(true);

        if (!HasFocus)
        {
            OnFocusGained();
        }

        ++hasFocusCount;
    }

    protected virtual void OnFocusGained() { }

    public void LostFocus(GameObject focusObject)
    {
        identityCanvasManager.gameObject.SetActive(false);

        if (!HasMultiFocus)
        {
            OnFocusLost();
        }

        --hasFocusCount;
    }

    protected virtual void OnFocusLost() { }
}