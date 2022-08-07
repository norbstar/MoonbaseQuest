using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public abstract class ButtonGroupPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] protected GameObject group;

        protected override List<UnityButton> ResolveButtons() => group.GetComponentsInChildren<UnityButton>().ToList();
    }
}