using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess
{
    public class AnnotationUIManager : MonoBehaviour
    {
        [SerializeField] float onsetDelay = 0.5f;

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

        public void OnClickButton() => Hide();

        private void Hide()
        {
            StopCoroutine(coroutine);
            asset.SetActive(false);
        }
    }
}