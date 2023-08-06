using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

using static Enum.HandEnums;

public class HandActionCanvasManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] Image hold;
    [SerializeField] Image pinch;
    [SerializeField] Image point;
    [SerializeField] Image claw;
    [SerializeField] Image hover;

    [Header("Color")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightColor;

    private Action lastAction;

    void Awake() => SetAction(Action.None);

    // void Start() => SetState(State.Grip);

    public void SetAction(Action action)
    {
        Log($"{gameObject.name} {className} SetAction Action: {action}");
        if (action == lastAction) return;

        this.lastAction = action;

#if false
        grip.color = defaultColor;
        pinch.color = defaultColor;
        point.color = defaultColor;
        claw.color = defaultColor;
        hover.color = defaultColor;
        
        switch (state)
        {
            case State.Grip:
                grip.color = highlightColor;
                break;

            case State.Pinch:
                pinch.color = highlightColor;
                break;

            case State.Point:
                point.color = highlightColor;
                break;

            case State.Claw:
                claw.color = highlightColor;
                break;

            case State.Hover:
                hover.color = highlightColor;
                break;
        }
#endif

        hold.color = (action.HasFlag(Action.Holding)) ? highlightColor : defaultColor;
        pinch.color = (action.HasFlag(Action.Pinching)) ? highlightColor : defaultColor;
        point.color = (action.HasFlag(Action.Pointing)) ? highlightColor : defaultColor;
        claw.color = (action.HasFlag(Action.Clawing)) ? highlightColor : defaultColor;
        hover.color = (action.HasFlag(Action.Hovering)) ? highlightColor : defaultColor;

        Log($"{gameObject.name} {className}.Action: {action}");
    }
}