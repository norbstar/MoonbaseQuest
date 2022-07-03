using UnityEngine;

namespace Chess
{
    public class ExitPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
            else if (selectedButton.name.Equals("Exit Button"))
            {
                Application.Quit();
            }
        }
    }
}