using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    public class DialogPanelUIManager : TextReceiver
    {
        [Header("Components")]
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI textUI;

        [Header("Config")]
        [SerializeField] float onsetDelay = 0.5f;

        public string Text
        {
            set
            {
                textUI.text = value;
                
                if (textUI.text.Length > 0)
                {
                    background.enabled = true;
                }
            }   
        }

        private void Hide()
        {
            background.enabled = false;
            textUI.text = default(string);
        }

        public override void OnText(string text)
        {
            if (text.Length > 0)
            {
                Text = text;
            }
            else
            {
                Hide();
            }
        }
    }
}