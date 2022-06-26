namespace Chess
{
    public class MainPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedObject.name.Equals("Exit Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Exit);
            }
        }
    }
}