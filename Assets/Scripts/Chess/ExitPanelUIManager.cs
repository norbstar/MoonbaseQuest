using UnityEngine;

namespace Chess
{
    public class ExitPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Forget It Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
            else if (selectedButton.name.Equals("Do It Button"))
            {
                Application.Quit();
            }
        }
    }
}