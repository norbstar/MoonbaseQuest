using UnityEngine;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class ConfirmationPanelUIManager : ButtonPanelUIManager
    {
        public enum Identity
        {
            Yes,
            No
        }

        public delegate void OnClickEvent(Identity identity);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton(UnityButton button)
        {
            base.OnClickButton(button);

            var name = button.name;

            if (name.Equals("No Button"))
            {
                EventReceived?.Invoke(Identity.No);
            }
            else if (name.Equals("Yes Button"))
            {
                EventReceived?.Invoke(Identity.Yes);
            }
        }
    }
}