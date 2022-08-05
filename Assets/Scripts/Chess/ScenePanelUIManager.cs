using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class ScenePanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] ButtonGroupManager selectManager;

        void OnEnable()
        {
            selectManager.EventReceived += OnSelectEvent;
        }

        void OnDisable() => selectManager.EventReceived -= OnSelectEvent;

        private void OnSelectEvent(List<UnityEngine.UI.Button> group) { }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Cancel Button"))
            {
                gameObject.SetActive(false);
            }
            else if (selectedButton.name.Equals("Apply Button"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}