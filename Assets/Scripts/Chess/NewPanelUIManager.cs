using UnityEngine;

namespace Chess
{
    public class NewPanelUIManager : BasePanelUIManager
    {
        [Header("Components")]
        [SerializeField] ToggleGroupManager interactionModeManager;
        [SerializeField] ToggleGroupManager playAsManager;
        [SerializeField] ToggleGroupManager playOrderManager;
        [SerializeField] ToggleGroupManager oppositionSkillLevelManager;
        
        public override void Start()
        {
            base.Start();

            if (onToggleSelectClip != null)
            {
                interactionModeManager.OnToggleClip = onToggleSelectClip;
                playAsManager.OnToggleClip = onToggleSelectClip;
                playOrderManager.OnToggleClip = onToggleSelectClip;
                oppositionSkillLevelManager.OnToggleClip = onToggleSelectClip;
            }
        }
        
        void OnEnable()
        {
            interactionModeManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);
            playAsManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Set);
            playOrderManager.SetByIndex((int) (((SettingsManager) SettingsManager.Instance).PlayFirst ? 0 : 1));
            oppositionSkillLevelManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Skill);
        }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Scene Button"))
            {
                // TODO
            }
            else if (selectedButton.name.Equals("Launch Button"))
            {
                // TODO
            }
        }
    }
}