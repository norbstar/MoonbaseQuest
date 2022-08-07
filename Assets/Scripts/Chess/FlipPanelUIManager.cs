using System.Collections.Generic;

using UnityEngine;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class FlipPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] UnityButton offButton;
        [SerializeField] UnityButton onButton;

        protected override List<UnityButton> ResolveButtons() => new List<UnityButton>() { offButton, onButton };

        public override void OnClickButton(UnityButton button)
        {
            base.OnClickButton(button);

            var name = button.name;

            if (name.Equals("Off Button "))
            {
                // TODO
            }
            else if (name.Equals("On Button "))
            {
                // TODO
            }
        }
    }
}