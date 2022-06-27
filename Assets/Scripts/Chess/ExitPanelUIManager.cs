using UnityEngine;

namespace Chess
{
    public class ExitPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Confirm Button"))
            {
                Application.Quit();
            }
            else if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}