using UnityEngine;

using UnityEngine.XR;

using static Enum.ControllerEnums;

[RequireComponent(typeof(BoxCollider))]
public class HybridHandButtonManager : SimpleHandButtonManager, IFocus
{
    private new BoxCollider collider;
    private bool hasFocus;
    private Vector3 focalPoint;
    private bool lastIsPressed;

    public override void Awake()
    {
        base.Awake();
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        collider = GetComponent<BoxCollider>() as BoxCollider;
    }

    void OnEnable()
    {
        HandController.InputChangeEventReceived += OnActuation;
    }

    void OnDisable()
    {
        HandController.InputChangeEventReceived -= OnActuation;
    }

    public void GainedFocus(GameObject gameObject, Vector3 focalPoint)
    {
        this.focalPoint = focalPoint;
        hasFocus = true;
    }

    // NOTE that this focus model does not accomodate multiple source of concurrent focus,
    // as once one looses focus, all that remain are untracked
    public void LostFocus(GameObject gameObject) => hasFocus = false;

    private void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
    {
        if (!hasFocus)
        {
            if (lastIsPressed) ResetButton();
            return;
        }

        bool isPressed = actuation.HasFlag(Enum.ControllerEnums.Input.Trigger);
        
        if (isPressed && !lastIsPressed)
        {
            HandleOnEnterEvent(0);
        }
        else if (lastIsPressed)
        {
            HandleOnExitEvent(0);
        }

        lastIsPressed = isPressed;
    }
}