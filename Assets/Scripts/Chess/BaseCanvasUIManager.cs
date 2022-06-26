using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess
{
    public class BaseCanvasUIManager : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;
        [SerializeField] protected AudioClip onSelectClip;

        protected GameObject selectedObject;

        public void OnPointerEnter(BaseEventData eventData)
        {
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1f);

            AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);

            selectedObject = gameObject;
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var gameObject = ((PointerEventData) eventData).pointerEnter;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            selectedObject = null;
        }

        public virtual void OnClickButton() => AudioSource.PlayClipAtPoint(onSelectClip, Vector3.zero, 1.0f);
    }
}