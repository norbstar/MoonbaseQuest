using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashlightInteractableManager : FocusableInteractableManager
{
    public enum State
    {
        Off,
        On
    }

    [Header("Components")]
    [SerializeField] GameObject spotlight;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip buttonClip;

    [Header("Config")]
    [SerializeField] State startState;

    private State state;

    // Start is called before the first frame update
    void Start()
    {
        state = startState;
        spotlight.SetActive(state == State.On);
    }

    public void OnActivated(ActivateEventArgs args)
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position, 1.0f);
        state = (state == State.Off) ? State.On : State.Off;
        spotlight.SetActive(state == State.On);
    }
}