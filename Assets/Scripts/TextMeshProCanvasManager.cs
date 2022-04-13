using UnityEngine;

using TMPro;

public class TextMeshProCanvasManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI textUI;

    private string text;

    public string Text
    {
        get
        {
            return text;
        }
        
        set
        {
            textUI.text = value;
        }
    }

    public void Reset() => textUI.text = "0 0";
}