using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    [RequireComponent(typeof(TextReceiver))]
    public class DialogPanelUIManager : MonoBehaviour
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

        private TextReceiver textReceiver;

        void Awake() => ResolveDependencies();
        
        private void ResolveDependencies() => textReceiver = GetComponent<TextReceiver>() as TextReceiver;

        void OnEnable() => textReceiver.EventReceived += OnText;

        void OnDisable() => textReceiver.EventReceived -= OnText;

        private void Hide()
        {
            background.enabled = false;
            textUI.text = default(string);
        }

        private void OnText(string text)
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