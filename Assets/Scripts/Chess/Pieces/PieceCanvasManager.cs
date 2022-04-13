using UnityEngine;

using TMPro;

namespace Chess
{
    public class PieceCanvasManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textUI;

        void Awake()
        {
            textUI.text = string.Empty;
        }

        public string Text
        {
            get
            {
                return textUI.text;
            }
            
            set
            {
                textUI.text = value;
            }
        }
    }
}