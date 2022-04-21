using UnityEngine;

using TMPro;

namespace Chess.Button
{
    public class ReassignableButtonEventManager : ButtonEventManager
    {
        [SerializeField] TextMeshProUGUI textUI;

        public ButtonId Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
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