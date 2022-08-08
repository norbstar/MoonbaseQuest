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

        protected override void OnPointerDownOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor) => coroutine = StartCoroutine(ManageProgressCoroutine());

        protected override void OnPointerUpOverride(PointerEventData eventData, GameObject gameObject, XRRayInteractor rayInteractor) => CancelProgress();

        protected override void OnPointerExitOverride(PointerEventData eventData, GameObject gameObject)
        {
            base.OnPointerExitOverride(eventData, gameObject);
            CancelProgress();
        }

        private void CancelProgress()
        {
            Debug.Log("CancelProgress");
            StopCoroutine(coroutine);
            progress.fillAmount = 0f;
        }

        public override void OnClickButton(UnityButton button)
        {
            if (onSelectClip != null)
            {
                AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
            }

            progress.fillAmount = 0f;
        }

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
        }
    }
}