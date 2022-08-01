using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    public class DialogPanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI textUI;

        public string Text
        {
            get
            {
                return textUI.text;
            }
            
            set
            {
                textUI.text = value;
                
                if (textUI.text.Length > 0)
                {
                    background.enabled = true;
                }
            }
        }

        public void Reset()
        {
            background.enabled = false;
            textUI.text = default(string);
        }
    }
}