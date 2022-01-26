using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

public class HandGestureCanvasManager : MonoBehaviour
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Flags]
    public enum Gesture
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

     [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private Gesture lastGesture;

    public void SetGestureState(Gesture gesture)
    {
        if (gesture == lastGesture) return;

        this.lastGesture = gesture;

#if false
        grip.color = defaultColor;
        pinch.color = defaultColor;
        point.color = defaultColor;
        claw.color = defaultColor;
        hover.color = defaultColor;
        
        switch (gesture)
        {
            case Gesture.Grip:
                grip.color = highlightColor;
                break;

            case Gesture.Pinch:
                pinch.color = highlightColor;
                break;

            case Gesture.Point:
                point.color = highlightColor;
                break;

            case Gesture.Claw:
                claw.color = highlightColor;
                break;

            case Gesture.Hover:
                hover.color = highlightColor;
                break;
        }
#endif

        grip.color = (gesture.HasFlag(Gesture.Grip)) ? highlightColor : defaultColor;
        pinch.color = (gesture.HasFlag(Gesture.Pinch)) ? highlightColor : defaultColor;
        point.color = (gesture.HasFlag(Gesture.Point)) ? highlightColor : defaultColor;
        claw.color = (gesture.HasFlag(Gesture.Claw)) ? highlightColor : defaultColor;
        hover.color = (gesture.HasFlag(Gesture.Hover)) ? highlightColor : defaultColor;

        Log($"{gameObject.name} {className}.State:{gesture}");
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}