namespace Chess
{
    public class ShortcutPanelUIManager : BasePanelUIManager
    {
        public enum Button
        {
            Game,
            Settings,
            About,
            Exit
        }

        public delegate void OnClickEvent(Button button);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Game Button"))
            {
                EventReceived?.Invoke(Button.Game);
            }
            else if (selectedButton.name.Equals("Settings Button"))
            {
                EventReceived?.Invoke(Button.Settings);
            }
            else if (selectedButton.name.Equals("About Button"))
            {
                EventReceived?.Invoke(Button.About);
            }
            else if (selectedButton.name.Equals("Exit Button"))
            {
                EventReceived?.Invoke(Button.Exit);
            }
        }
    }
}