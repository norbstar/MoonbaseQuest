using UnityEngine;
using UnityEngine.UI;

public class HandGestureCanvasManager : MonoBehaviour
{
    public enum Gesture
    {
        Grip,
        Pinch,
        Point,
        Claw,
        Hover
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

    private Gesture lastGesture;

    public void SetState(Gesture gesture)
    {
        if (gesture == lastGesture) return;

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

        lastGesture = gesture;
    }
}