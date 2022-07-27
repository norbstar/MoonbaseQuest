using System;

using UnityEngine;

using TMPro;

public class FadeValuePanelUI : MonoBehaviour
{
    [SerializeField] FadeOutImage fadeOutImage;
    [SerializeField] TextMeshProUGUI textUI;
 
    // public string Text { set { textUI.text = value; } }

    // Update is called once per frame
    void Update() => textUI.text = fadeOutImage.FractionComplete.ToString("F2");
}