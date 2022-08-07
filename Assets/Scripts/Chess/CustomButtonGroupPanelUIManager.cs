using System.Collections.Generic;

using UnityEngine;
using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public abstract class CustomButtonGroupPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] protected List<UnityButton> buttons;

        public override void Awake()
        {
            base.Awake();
            ResolveDependencies();
        }

        private void ResolveDependencies() => Buttons = buttons;
    }
}