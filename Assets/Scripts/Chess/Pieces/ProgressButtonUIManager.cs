using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class ProgressButtonUIManager : ButtonUIManager
    {
        [Header("Components")]
        [SerializeField] Image progress;

        [Header("Config")]
        [SerializeField] float delaySec = 0.5f;

        private Coroutine coroutine;
        private float fractionComplete;

        public override void Awake()
        {
            base.Awake();
            progress.fillAmount = 0f;
        }

        protected override void OnPointerDown(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            base.OnPointerDown(eventData, gameObject, rayInteractor);
            coroutine = StartCoroutine(ManageProgressCoroutine());
        }

        protected override void OnPointerUp(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            base.OnPointerUp(eventData, gameObject, rayInteractor);
            Cancel();
        }

        protected override void OnPointerExit(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor)
        {
            base.OnPointerExit(eventData, gameObject, rayInteractor);
            Cancel();
        }

        private void Cancel()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            NotifyReceivers(string.Empty);

            if (deselectOnSelect)
            {
                StartCoroutine(DeselectCoroutine(deselectionDelay));
            }

            progress.fillAmount = 0f;
        }

        public override void OnClickButton(UnityButton button) => progress.fillAmount = 0f;

        private IEnumerator ManageProgressCoroutine()
        {
            bool complete = false;           
            float startTime = Time.time;            
            float endTime = startTime + delaySec;

            while (!complete)
            {
                fractionComplete = (Time.time - startTime) / delaySec;
                complete = (fractionComplete >= 1f);
                
                progress.fillAmount = fractionComplete;

                yield return null;
            }

            if (onSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
            }

            PostEvent(Event.OnClick);
        }
    }
}