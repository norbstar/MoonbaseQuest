using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;

namespace Chess
{
    public class AnnotationUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] TextMeshProUGUI textUI;

        [Header("Config")]
        [SerializeField] float onsetDelay = 0.5f;

        public string Text { get { return textUI.text; } }

        private GameObject asset;
        private Coroutine coroutine;

        void Start()
        {
            asset = transform.GetChild(0).gameObject;
        }

        public void OnPointerEnter(BaseEventData eventData) => coroutine = StartCoroutine(AnnotateCoroutine());

        private IEnumerator AnnotateCoroutine()
        {
            yield return new WaitForSeconds(onsetDelay);
            asset.SetActive(true);
        }

        public void OnPointerExit(BaseEventData eventData) => Hide();

        public void Hide()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            asset.SetActive(false);
        }
    }
}