using System.Linq;

using UnityEngine;
using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public abstract class ButtonGroupPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] protected GameObject group;

        public override void Awake()
        {
            base.Awake();
            ResolveDependencies();
        }

        private void ResolveDependencies() => Buttons = group.GetComponentsInChildren<UnityButton>().ToList();
    }
}