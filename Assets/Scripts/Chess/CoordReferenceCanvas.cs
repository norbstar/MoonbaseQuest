using UnityEngine;

using TMPro;

namespace Chess
{
    public class CoordReferenceCanvas : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textUI;
        public string TextUI
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