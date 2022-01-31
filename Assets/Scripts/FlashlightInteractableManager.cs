using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashlightInteractableManager : FocusableInteractableManager, IGesture
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

    // Start is called before the first frame update
    void Start()
    {
        State = startState;
        spotlight.SetActive(state == ActiveState.On);
    }

    public void OnGesture(HandController.Gesture gesture, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnGesture:Gesture : {gesture} Value : {value}");

        switch (gesture)
        {
            case HandController.Gesture.Button_AX:
                AlternateLight();
                break;
        }
    }

    private void AlternateLight()
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position, 1.0f);
        State = (state == ActiveState.Off) ? ActiveState.On : ActiveState.Off;
    }
}