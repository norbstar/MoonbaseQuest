using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class AcceptanceTextAreaUIManager : TextAreaUIManager
    {
        [Header("Advanced Components")]
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] ButtonUIManager acceptButton;

        [Header("Advanced Config")]
        [SerializeField] float fadeDurationSec = 0.5f;

        public enum Identity
        {
            AcceptButton
        }

        public delegate void OnClickEvent(Identity identity);
        public event OnClickEvent EventReceived;
        
        private CanvasGroup canvasGroup;
        private bool acceptButtonShown;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            canvasGroup = acceptButton.gameObject.GetComponent<CanvasGroup>() as CanvasGroup;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            acceptButton.EventReceived += OnAcceptButtonEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            acceptButton.EventReceived -= OnAcceptButtonEvent;
        }

        private void OnAcceptButtonEvent(GameObject gameObject, ButtonUIManager.Event evt)
        {
            switch (evt)
            {
                case ButtonUIManager.Event.OnClick:
                    EventReceived?.Invoke(Identity.AcceptButton);
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if ((!acceptButtonShown) && (System.Math.Round(scrollRect.verticalNormalizedPosition, 2) == 0f))
            {
                StartCoroutine(ShowAcceptButtonCoroutine());
                acceptButtonShown = true;
            }
        }

        private IEnumerator ShowAcceptButtonCoroutine()
        {
            bool complete = false;           
            float startTime = Time.time;            

            if (canvasGroup != null)
            {
                while (!complete)
                {
                    float fractionComplete = (Time.time - startTime) / fadeDurationSec;
                    complete = (fractionComplete >= 1f);
                    
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, fractionComplete);
                    yield return null;
                }

                canvasGroup.interactable = true;
            }
        }
    }
}