using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class ShortcutPanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] ButtonGroupManager shortcutManager;

        public override void Start()
        {
            base.Start();

            if (onButtonSelectClip != null)
            {
                if (shortcutManager.OnSelectClip == null)
                {
                    shortcutManager.OnSelectClip = onButtonSelectClip;
                }
            }
        }

        void OnEnable()
        {
            shortcutManager.EventReceived += OnSelectEvent;
        }

        void OnDisable() => shortcutManager.EventReceived -= OnSelectEvent;

        private void OnSelectEvent(List<UnityEngine.UI.Button> group) { }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Settings Button"))
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