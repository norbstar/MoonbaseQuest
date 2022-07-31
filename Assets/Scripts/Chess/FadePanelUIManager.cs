namespace Chess
{
    public class FadePanelUIManager : BasePanelUIManager
    {
        public enum Button
        {
            FadeIn,
            FadeOut
        }

        public delegate void OnClickEvent(Button button);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Fade In Button"))
            {
                EventReceived?.Invoke(Button.FadeIn);
            }
            else if (selectedButton.name.Equals("Fade Out Button"))
            {
                EventReceived?.Invoke(Button.FadeOut);
            }
        }
    }
}