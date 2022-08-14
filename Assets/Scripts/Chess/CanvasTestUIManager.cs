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
            SliderPanelUIManager.EventReceived += OnSliderEvent;
        }

        void OnDisable()
        {
            PlayModePanelUIManager.EventReceived -= OnPlayModeEvent;
            FadePanelUIManager.EventReceived -= OnFadeEvent;
            ShortcutPanelUIManager.EventReceived -= OnShortcutEvent;
            ConfirmationPanelUIManager.EventReceived -= OnConfirmationEvent;
            SliderPanelUIManager.EventReceived -= OnSliderEvent;
        }

        private void OnPlayModeEvent(PlayModePanelUIManager.Identity identity) { }

        private void OnFadeEvent(FadePanelUIManager.Identity identity) { }

        private void OnShortcutEvent(ShortcutPanelUIManager.Identity identity)
        {
            switch (identity)
            {
                case ShortcutPanelUIManager.Identity.Game:
                    break;

                case ShortcutPanelUIManager.Identity.Settings:
                    break;

                case ShortcutPanelUIManager.Identity.About:
                    break;

                case ShortcutPanelUIManager.Identity.Exit:
                    confirmationPanel.gameObject.SetActive(true);
                    break;
            }
        }

        private void OnConfirmationEvent(ConfirmationPanelUIManager.Identity identity)
        {
            switch (identity)
            {
                case ConfirmationPanelUIManager.Identity.No:
                    confirmationPanel.gameObject.SetActive(false);
                    break;

                case ConfirmationPanelUIManager.Identity.Yes:
                    Application.Quit();
                    break;
            }
        }

        private void OnSliderEvent(float value) { }
    }
}