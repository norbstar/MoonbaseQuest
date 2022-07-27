using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class FadePanelUIManager : BasePanelUIManager
    {
        [Header("Custom Config")]
        [SerializeField] ImageFader imageFader;

        private void OnSelectEvent(List<UnityEngine.UI.Button> group) { }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Fade In Button"))
            {
                imageFader.FadeIn();
            }
            else if (selectedButton.name.Equals("Fade Out Button"))
            {
                imageFader.FadeOut();
            }
        }
    }
}