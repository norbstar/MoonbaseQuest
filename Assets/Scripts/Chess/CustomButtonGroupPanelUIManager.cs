using System.Collections.Generic;

using UnityEngine;
using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public abstract class CustomButtonGroupPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] protected List<UnityButton> buttonSet;

        protected override List<UnityButton> ResolveButtons() => buttonSet;
    }
}