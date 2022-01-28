using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractiveCanvasManager : MonoBehaviour
{
    [SerializeField] GameObject buttonA, buttonB, buttonC;
    [SerializeField] Image image;

    public void OnPointerEnter(BaseEventData eventData)
    {
        var gameObject = ((PointerEventData) eventData).pointerEnter;
        
        if (gameObject.transform.parent.name.Equals("Button A"))
        {
            buttonA.transform.localScale = new Vector3(1f, 1f, 0.5f);
        }
        else if (gameObject.transform.parent.name.Equals("Button B"))
        {
            buttonB.transform.localScale = new Vector3(1f, 1f, 0.5f);
        }
        else if (gameObject.transform.parent.name.Equals("Button C"))
        {
            buttonC.transform.localScale = new Vector3(1f, 1f, 0.5f);
        }
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        var gameObject = ((PointerEventData) eventData).pointerEnter;

        if (gameObject.transform.parent.name.Equals("Button A"))
        {
            buttonA.transform.localScale = new Vector3(1f, 1f, 0.25f);
        }
        else if (gameObject.transform.parent.name.Equals("Button B"))
        {
            buttonB.transform.localScale = new Vector3(1f, 1f, 0.25f);
        }
        else if (gameObject.transform.parent.name.Equals("Button C"))
        {
            buttonC.transform.localScale = new Vector3(1f, 1f, 0.25f);
        }
    }

    public void OnClickButton()
    {
        image.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
    }
}