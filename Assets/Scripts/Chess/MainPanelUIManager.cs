namespace Chess
{
    public class MainPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Exit Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Exit);
            }
        }
    }
}