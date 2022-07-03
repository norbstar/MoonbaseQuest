using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    public class SettingsPanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] ToggleGroupManager toggleGroupManager;
        // [SerializeField] TMP_Dropdown interactionModeDropdown;

        // Start is called before the first frame update
        // void Start()
        // {
        //     interactionModeDropdown.onValueChanged.AddListener(delegate {
        //         ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);
        //     });
        // }

        void OnEnable()
        {
            toggleGroupManager.EventReceived += OnToogleGroupEvent;
            // interactionModeDropdown.value = (int) ((SettingsManager) SettingsManager.Instance).InteractionMode;
            toggleGroupManager.TurnOnByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);
        }

        void OnDisable()
        {
            toggleGroupManager.EventReceived -= OnToogleGroupEvent;
        }

        private void OnToogleGroupEvent(List<Toggle> group) { }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
            else if (selectedButton.name.Equals("Confirm Button"))
            {
                // ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);

                int index = toggleGroupManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(index);
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}