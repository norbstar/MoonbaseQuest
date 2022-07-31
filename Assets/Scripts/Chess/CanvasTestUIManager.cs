using UnityEngine;

namespace Chess
{
    public class CanvasTestUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] PlayModePanelUIManager playModePanel;
        [SerializeField] FadePanelUIManager fadePanel;
        [SerializeField] ShortcutPanelUIManager shortcutsPanel;
        [SerializeField] ConfirmationPanelUIManager confirmationPanel;
        [SerializeField] SliderPanelUIManager volumePanel;

        void OnEnable()
        {
            PlayModePanelUIManager.EventReceived += OnPlayModeEvent;
            FadePanelUIManager.EventReceived += OnFadeEvent;
            ShortcutPanelUIManager.EventReceived += OnShortcutEvent;
            ConfirmationPanelUIManager.EventReceived += OnConfirmationEvent;
            SliderPanelUIManager.EventReceived += OnVolumeEvent;
        }

        void OnDisable()
        {
            PlayModePanelUIManager.EventReceived -= OnPlayModeEvent;
            FadePanelUIManager.EventReceived -= OnFadeEvent;
            ShortcutPanelUIManager.EventReceived -= OnShortcutEvent;
            ConfirmationPanelUIManager.EventReceived -= OnConfirmationEvent;
            SliderPanelUIManager.EventReceived -= OnVolumeEvent;
        }

        private void OnPlayModeEvent(PlayModePanelUIManager.Button button) { }

        private void OnFadeEvent(FadePanelUIManager.Button button) { }

        private void OnShortcutEvent(ShortcutPanelUIManager.Button button)
        {
            switch (button)
            {
                case ShortcutPanelUIManager.Button.Game:
                    break;

                case ShortcutPanelUIManager.Button.Settings:
                    break;

                case ShortcutPanelUIManager.Button.About:
                    break;

                case ShortcutPanelUIManager.Button.Exit:
                    confirmationPanel.gameObject.SetActive(true);
                    break;
            }
        }

        private void OnConfirmationEvent(ConfirmationPanelUIManager.Button button)
        {
            switch (button)
            {
                case ConfirmationPanelUIManager.Button.No:
                    confirmationPanel.gameObject.SetActive(false);
                    break;

                case ConfirmationPanelUIManager.Button.Yes:
                    Application.Quit();
                    break;
            }
        }

        private void OnVolumeEvent(float value) { }
    }
}