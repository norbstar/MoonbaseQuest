using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Conways
{
    [RequireComponent(typeof(PointerEventHandler))]
    [RequireComponent(typeof(Image))]
    public class ConwaysCellUIManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] Sprite unselected;
        [SerializeField] Sprite selected;

        public Vector2 Coord
        {
            get
            {
                return coord;
            }

            set
            {
                coord = value;
            }
        }

        private PointerEventHandler eventHandler;
        private Image image;
        private static bool isPointerDown;
        private bool inFocus, isSelected;
        private Vector2 coord;

         void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            eventHandler = GetComponent<PointerEventHandler>() as PointerEventHandler;
            image = GetComponent<Image>() as Image;
        }
        
        void OnEnable() => eventHandler.EventReceived += OnPointerEvent;

        void OnDisable() => eventHandler.EventReceived -= OnPointerEvent;

        private XRUIInputModule GetXRInputModule() => EventSystem.current.currentInputModule as XRUIInputModule;

        private bool TryGetXRRayInteractor(int pointerID, out XRRayInteractor rayInteractor)
        {
            var inputModule = GetXRInputModule();

            if (inputModule == null)
            {
                rayInteractor = null;
                return false;
            }

            rayInteractor = inputModule.GetInteractor(pointerID) as XRRayInteractor;
            return (rayInteractor != null);
        }

        private void OnPointerEvent(GameObject gameObject, PointerEventHandler.Event evt, PointerEventData eventData)
        {
            switch (evt)
            {
                case PointerEventHandler.Event.Enter:
                    OnPointerEnter(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Down:
                    OnPointerDown(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Up:
                    OnPointerUp(gameObject, eventData);
                    break;

                case PointerEventHandler.Event.Exit:
                    OnPointerExit(gameObject, eventData);
                    break;
            }
        }
        
        private void OnPointerEnter(GameObject gameObject, PointerEventData eventData)
        {
            inFocus = true;

            if (isPointerDown)
            {
                IsSelected = !isSelected;
            }
        }

        private void OnPointerDown(GameObject gameObject, PointerEventData eventData)
        {
            isPointerDown = true;

            if (inFocus)
            {
                IsSelected = !isSelected;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                image.sprite = (isSelected) ? selected : unselected;
            }
        }

        private void OnPointerUp(GameObject gameObject, PointerEventData eventData) => isPointerDown = false;

        private void OnPointerExit(GameObject gameObject, PointerEventData eventData) => inFocus = false;
    }
}