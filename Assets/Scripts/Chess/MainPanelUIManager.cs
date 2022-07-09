namespace Chess
{
    public class MainPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Game Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Game);
            }
            else if (selectedButton.name.Equals("Settings Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Settings);
            }
            else if (selectedButton.name.Equals("Exit Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Exit);
            }
        }
    }
}