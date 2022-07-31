namespace Chess
{
    public class MainPanelUIManager : BasePanelUIManager
    {
        public enum Button
        {
            Game,
            Settings,
            Exit
        }

        public delegate void OnClickEvent(Button button);
        public static event OnClickEvent EventReceived;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Game Button"))
            {
                // canvasManager.ShowPanel(CanvasUIManager.PanelType.Game);
                EventReceived?.Invoke(Button.Game);
            }
            else if (selectedButton.name.Equals("Settings Button"))
            {
                // canvasManager.ShowPanel(CanvasUIManager.PanelType.Settings);
                EventReceived?.Invoke(Button.Settings);
            }
            else if (selectedButton.name.Equals("Exit Button"))
            {
                // canvasManager.ShowPanel(CanvasUIManager.PanelType.Exit);
                EventReceived?.Invoke(Button.Exit);
            }
        }
    }
}