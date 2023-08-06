using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using static Enum.ControllerEnums;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashlightInteractableManager : FocusableInteractableManager, IInputChange
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

    public void OnInputChange(Enum.ControllerEnums.Input input, InputDeviceCharacteristics characteristics, object value = null)
    {
        Log($"{Time.time} {gameObject.name} {className} OnInputChange:Input : {input} Value : {value}");

        if (!IsHeld) return;

        if (input.HasFlag(Enum.ControllerEnums.Input.Button_AX))
        {
            AlternateState();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        Log($"{Time.time} {gameObject.name} {className} OnSelectExited:Controller : {controller.name}");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.5f, transform.forward, 0f);

        bool isHovering = false;

        foreach (RaycastHit hit in hits)
        {
            GameObject gameObject = hit.collider.gameObject;
            
            if (gameObject.TryGetComponent<XRSocketInteractor>(out XRSocketInteractor interactor))
            {
                List<IXRHoverInteractable> interactables = interactor.interactablesHovered;

                foreach (IXRHoverInteractable interactable in interactables)
                {
                    if (GameObject.ReferenceEquals(interactable.transform.gameObject, this.interactable.gameObject))
                    {
                        isHovering = true;
                    }
                }
            }
        }

        if (!isHovering)
        {
            Log($"{Time.time} {gameObject.name} {className} OnSelectExited:Revert Interaction Mask");
            this.interactable.interactionLayers = InteractionLayerMask.GetMask(new string[] { "Default", "Flashlight" });
        }
    }

    private void AlternateState()
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position, 1.0f);
        State = (state == ActiveState.Off) ? ActiveState.On : ActiveState.Off;
    }
}