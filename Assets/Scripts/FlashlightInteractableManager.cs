using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashlightInteractableManager : FocusableInteractableManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public enum ActiveState
    {
        Off,
        On
    }

    [Header("Components")]
    [SerializeField] GameObject spotlight;

    [Header("Audio")]
    [SerializeField] AudioClip buttonClip;

    [Header("Config")]
    [SerializeField] ActiveState startState;

    private ActiveState state;

    // Start is called before the first frame update
    void Start()
    {
        State = startState;
        spotlight.SetActive(state == ActiveState.On);
    }

    public void OnActivated(ActivateEventArgs args)
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position, 1.0f);
        State = (state == ActiveState.Off) ? ActiveState.On : ActiveState.Off;
    }

    public ActiveState State {
        get
        {
            return state;
        }

        set
        {
            state = value;
            spotlight.SetActive(state == ActiveState.On);
        }
    }
}