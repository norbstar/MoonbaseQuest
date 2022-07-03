namespace Chess
{
    public class MainPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Play Button"))
            {
                // TODO
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