using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ImageFader))]    
    public class ImageFaderManager : MonoBehaviour
    {
        private ImageFader imageFader;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => imageFader = GetComponent<ImageFader>() as ImageFader;

        void OnEnable() => FadePanelUIManager.EventReceived += OnFadePanelUIEvent;

        void OnDisable() => FadePanelUIManager.EventReceived -= OnFadePanelUIEvent;

        public void OnFadePanelUIEvent(FadePanelUIManager.Button button)
        {
            switch (button)
            {
                case FadePanelUIManager.Button.FadeIn:
                    imageFader.FadeIn();
                    break;

                case FadePanelUIManager.Button.FadeOut:
                    imageFader.FadeOut();
                    break;
            }
        }
    }
}