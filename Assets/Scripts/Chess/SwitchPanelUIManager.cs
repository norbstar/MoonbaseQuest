using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class SwitchPanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<SwitchUIManager> switches;

        [Header("Config")]
        [SerializeField] float durationSec = 0.5f;

        public enum Identity
        {
            Off,
            On
        }

        public delegate void OnSelectEvent(Identity identity);
        public event OnSelectEvent EventReceived;

        void OnEnable()
        {
            foreach (SwitchUIManager instance in switches)
            {
                instance.EventReceived += OnEvent;
            }
        }

        void OnDisable()
        {
            foreach (SwitchUIManager instance in switches)
            {
                instance.EventReceived -= OnEvent;
            }
        }

        public void OnEvent(SwitchUIManager.Identity identity)
        {
            // TODO
        }

        protected void OnPointerEnterOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            // var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;
            Debug.Log($"OnPointerEnterOverride {gameObject.name}");

            if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
            {
                var scrollRect = rootGameObject.GetComponent<ScrollRect>() as ScrollRect;
                
                if (gameObject.name.Equals("Off Button"))
                {
                    StartCoroutine(ScrollToBottomCoroutine(scrollRect));
                }
                else if (gameObject.name.Equals("On Button"))
                {
                    StartCoroutine(ScrollToTopCoroutine(scrollRect));
                }
            }
        }

        protected void OnPointerExitOverride(PointerEventData eventData, GameObject gameObject)
        {
            // var manager = gameObject.GetComponent<ButtonUIManager>() as ButtonUIManager;
            Debug.Log($"OnPointerExitOverride {gameObject.name}");

            if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
            {
                var scrollRect = rootGameObject.GetComponent<ScrollRect>() as ScrollRect;
                
                if (gameObject.name.Equals("Off Button"))
                {
                    StartCoroutine(ScrollToBottomCoroutine(scrollRect));
                }
                else if (gameObject.name.Equals("On Button"))
                {
                    StartCoroutine(ScrollToTopCoroutine(scrollRect));
                }
            }
        }

        private bool flipped = false;

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

        public void OnClickButton(UnityButton button)
        {
            var name = button.name;
            // StartCoroutine(ScrollToBottom());
            // scrollRect.ScrollToBottom();

            if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
            {
                var scrollRect = rootGameObject.GetComponent<ScrollRect>() as ScrollRect;
                scrollRect.verticalNormalizedPosition = 1;
            }

            // if (name.Equals("Flip Button"))
            // {
            //     float y = (flipped) ? 0f : 4.8f;
            //     scrollGroup.localPosition = new Vector3(0f, y, 0f);

            //     if (flipped)
            //     {
            //         scrollRect.ScrollToTop();
            //     }
            //     else
            //     {
            //         scrollRect.ScrollToBottom();
            //     }
            // }

            flipped = !flipped;
        }
    }
}