using UnityEngine;

public abstract class FocusManager : MonoBehaviour, IFocus
{
    public bool HasFocus { get { return hasFocusCount > 0; } }
    public bool HasMultiFocus { get { return hasFocusCount > 1; } }

    private int hasFocusCount;

    public void GainedFocus(GameObject focusObject)
    {
        // Debug.Log($"{gameObject.name}.Focus:GainedFocus");

        if (!HasFocus)
        {
            OnFocusGained();
        }

        ++hasFocusCount;
    }

    protected abstract void OnFocusGained();

    public void LostFocus(GameObject focusObject)
    {
        // Debug.Log($"{gameObject.name}.Focus:LostFocus");

        if (!HasMultiFocus)
        {
            OnFocusLost();
        }

        --hasFocusCount;
    }

    protected abstract void OnFocusLost();
}
