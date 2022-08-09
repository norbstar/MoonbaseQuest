using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class SwitchUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] ButtonUIManager button;
        [SerializeField] ButtonUIManager altButton;

        [Header("Config")]
        [SerializeField] float durationSec = 0.5f;

        public enum Identity
        {
            Button,
            AltButton
        }

        public delegate void OnSelectEvent(Identity identity);
        public event OnSelectEvent EventReceived;

        void OnEnable()
        {
            button.EventReceived += OnButtonEvent;
            altButton.EventReceived += OnAltButtonEvent;
        }

        void OnDisable()
        {
            button.EventReceived -= OnButtonEvent;
            altButton.EventReceived -= OnAltButtonEvent;
        }

        private void OnButtonEvent(GameObject gameObject, ButtonUIManager.Event evt)
        {
            Debug.Log($"OnButtonEvent GameObject : {gameObject.name} Event : {evt}");
            
            if (TryGet.TryGetRootResolver(button.gameObject, out GameObject rootGameObject))
            {
                var scrollRect = rootGameObject.GetComponent<ScrollRect>() as ScrollRect;
                
                switch (evt)
                {
                    case ButtonUIManager.Event.OnPointerEnter:
                        StartCoroutine(ScrollToBottomCoroutine(scrollRect));
                        break;

                    // case ButtonUIManager.Event.OnPointerExit:
                    //     StartCoroutine(ScrollToTopCoroutine(scrollRect));
                    //     break;

                    case ButtonUIManager.Event.OnClick:
                        break;
                }
            }
        }

        private void OnAltButtonEvent(GameObject gameObject, ButtonUIManager.Event evt)
        {
            Debug.Log($"OnAltButtonEvent GameObject : {gameObject.name} Event : {evt}");

            if (TryGet.TryGetRootResolver(button.gameObject, out GameObject rootGameObject))
            {
                var scrollRect = rootGameObject.GetComponent<ScrollRect>() as ScrollRect;
                
                switch (evt)
                {
                    // case ButtonUIManager.Event.OnPointerEnter:
                    //     StartCoroutine(ScrollToBottomCoroutine(scrollRect));
                    //     break;

                    case ButtonUIManager.Event.OnPointerExit:

                        StartCoroutine(ScrollToTopCoroutine(scrollRect));
                        break;

                    case ButtonUIManager.Event.OnClick:
                        break;
                }
            }
        }

        private IEnumerator ScrollToTopCoroutine(ScrollRect scrollRect)
        {
            bool complete = false;           
            float startTime = Time.time;            
            float endTime = startTime + durationSec;

            while (!complete)
            {
                float fractionComplete = (Time.time - startTime) / durationSec;
                complete = (fractionComplete >= 1f);
                
                scrollRect.verticalNormalizedPosition = fractionComplete;
                
                yield return null;
            }
        }

        private IEnumerator ScrollToBottomCoroutine(ScrollRect scrollRect)
        {
            bool complete = false;           
            float startTime = Time.time;            
            float endTime = startTime + durationSec;

            while (!complete)
            {
                float fractionComplete = (Time.time - startTime) / durationSec;
                complete = (fractionComplete >= 1f);
                
                scrollRect.verticalNormalizedPosition = 1f - fractionComplete;
                
                yield return null;
            }
        }
    }
}