using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractiveCanvasManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject buttonA;
    [SerializeField] GameObject buttonB;
    [SerializeField] GameObject buttonC;
    [SerializeField] Image image;

    [Header("Audio")]
    [SerializeField] AudioClip onHoverClip;

    public void OnPointerEnter(BaseEventData eventData)
    {
        var gameObject = ((PointerEventData) eventData).pointerEnter;
        
        if (gameObject.transform.parent.name.Equals("Button A"))
        {
            buttonA.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (gameObject.transform.parent.name.Equals("Button B"))
        {
            buttonB.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (gameObject.transform.parent.name.Equals("Button C"))
        {
            buttonC.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        AudioSource.PlayClipAtPoint(onHoverClip, transform.position, 1.0f);
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
        if (image == null) return;

        image.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
    }
}