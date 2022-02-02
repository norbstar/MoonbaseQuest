using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

public class HandStateCanvasManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Flags]
    public enum State
    {
        None = 0,
        Grip = 1,
        Pinch = 2,
        Point = 4,
        Claw = 8,
        Hover = 16
    }

     [Header("Components")]
     [SerializeField] Image grip;
     [SerializeField] Image pinch;
     [SerializeField] Image point;
     [SerializeField] Image claw;
     [SerializeField] Image hover;

     [Header("Color")]
     [SerializeField] Color defaultColor;
     [SerializeField] Color highlightColor;

    private State lastState;

    public void SetState(State state)
    {
        if (state == lastState) return;

        this.lastState = state;

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

        grip.color = (state.HasFlag(State.Grip)) ? highlightColor : defaultColor;
        pinch.color = (state.HasFlag(State.Pinch)) ? highlightColor : defaultColor;
        point.color = (state.HasFlag(State.Point)) ? highlightColor : defaultColor;
        claw.color = (state.HasFlag(State.Claw)) ? highlightColor : defaultColor;
        hover.color = (state.HasFlag(State.Hover)) ? highlightColor : defaultColor;

        Log($"{gameObject.name} {className}.State:{state}");
    }
}