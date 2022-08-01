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

        public void OnFadePanelUIEvent(FadePanelUIManager.Identity identity)
        {
            switch (identity)
            {
                case FadePanelUIManager.Identity.FadeIn:
                    imageFader.FadeIn();
                    break;

                case FadePanelUIManager.Identity.FadeOut:
                    imageFader.FadeOut();
                    break;
            }
        }
    }
}