using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class AdvancedTextAreaUIManager : TextAreaUIManager
    {
        [Header("Advanced Components")]
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] ButtonUIManager topButton;
        [SerializeField] ButtonUIManager bottomButton;

        [Header("Advanced Config")]
        [SerializeField] float durationSec = 0.25f;

        public enum Identity
        {
            TopButton,
            BottomButton
        }

        public delegate void OnClickEvent(Identity identity);
        public event OnClickEvent EventReceived;

        private Coroutine coroutine;

        protected override void OnEnable()
        {
            base.OnEnable();

            topButton.EventReceived += OnTopButtonEvent;
            bottomButton.EventReceived += OnBottomButtonEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            topButton.EventReceived -= OnTopButtonEvent;
            bottomButton.EventReceived -= OnBottomButtonEvent;
        }

        private void OnTopButtonEvent(GameObject gameObject, ButtonUIManager.Event evt)
        {
            switch (evt)
            {
                case ButtonUIManager.Event.OnClick:
                    ScrollToTop();
                    EventReceived?.Invoke(Identity.TopButton);
                    break;
            }
        }

        private void OnBottomButtonEvent(GameObject gameObject, ButtonUIManager.Event evt)
        {
            switch (evt)
            {
                case ButtonUIManager.Event.OnClick:
                    ScrollToBottom();
                    EventReceived?.Invoke(Identity.BottomButton);
                    break;
            }
        }

        private void ScrollToTop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(ScrollToTopCoroutine());
        }

        private IEnumerator ScrollToTopCoroutine()
        {
            bool complete = false;
            float startTime = Time.time;
            float startVertical = scrollRect.verticalNormalizedPosition;

            while (!complete)
            {
                float fractionComplete = ((Time.time - startTime) / durationSec) + startVertical;
                complete = (fractionComplete >= 1f);
                
                scrollRect.verticalNormalizedPosition = fractionComplete;
                
                yield return null;
            }
        }

        private void ScrollToBottom()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(ScrollToBottomCoroutine());
        }

        private IEnumerator ScrollToBottomCoroutine()
        {
            bool complete = false;
            float startTime = Time.time;
            float startVertical = scrollRect.verticalNormalizedPosition;

            while (!complete)
            {
                float fractionComplete = ((Time.time - startTime) / durationSec) + (1f - startVertical);
                complete = (fractionComplete >= 1f);
                
                scrollRect.verticalNormalizedPosition = 1f - fractionComplete;
                
                yield return null;
            }
        }
    }
}