using UnityEngine;

namespace Chess
{
    public class LaunchCanvasUIManager : BaseCanvasUIManager
    {
        [SerializeField] CanvasUIManager canvasManger;

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedObject.name.Equals("Start Button"))
            {
                // TODO
            }
            else if (selectedObject.name.Equals("Exit Button"))
            {
                canvasManger.ShowPanel(CanvasUIManager.PanelType.Exit);
            }
        }
    }
}