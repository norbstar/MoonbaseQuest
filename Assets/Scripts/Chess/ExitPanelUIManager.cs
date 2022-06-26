using UnityEngine;

namespace Chess
{
    public class ExitPanelUIManager : BasePanelUIManager
    {
        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedObject.name.Equals("Confirm Button"))
            {
                Application.Quit();
            }
            else if (selectedObject.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}