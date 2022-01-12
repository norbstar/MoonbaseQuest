using UnityEngine;

using TMPro;

public class IdentityCanvasManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI identityTextUI;

    public string IdentityText {
        set
        {
            identityTextUI.text = value;
        }
    }
}