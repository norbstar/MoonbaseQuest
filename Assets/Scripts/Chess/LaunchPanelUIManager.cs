namespace Chess
{
    public class LaunchPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Start Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}