using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class FadePanelUIManager : ButtonGroupPanelUIManager
    {
        public enum Identity
        {
            FadeIn,
            FadeOut
        }

        public delegate void OnClickEvent(Identity identity);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton(UnityButton button)
        {
            base.OnClickButton(button);

            var name = button.name;

            if (name.Equals("Fade In Button"))
            {
                EventReceived?.Invoke(Identity.FadeIn);
            }
            else if (name.Equals("Fade Out Button"))
            {
                EventReceived?.Invoke(Identity.FadeOut);
            }
        }
    }
}