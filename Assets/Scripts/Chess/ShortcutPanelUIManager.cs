using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class ShortcutPanelUIManager : BasePanelUIManager
    {
        private void OnSelectEvent(List<UnityEngine.UI.Button> group) { }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Game Button"))
            {
                // TODO
            }
            else if (selectedButton.name.Equals("Settings Button"))
            {
                // TODO
            }
            else if (selectedButton.name.Equals("About Button"))
            {
                // TODO
            }
            else if (selectedButton.name.Equals("Exit Button"))
            {
                Application.Quit();
            }
        }
    }
}