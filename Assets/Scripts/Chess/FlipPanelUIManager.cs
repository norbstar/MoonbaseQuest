using System.Collections.Generic;

using UnityEngine;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class FlipPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] UnityButton offButton;
        [SerializeField] UnityButton onButton;

        public override void Awake()
        {
            base.Awake();
            
            Buttons = new List<UnityButton>() { offButton, onButton };
        }

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