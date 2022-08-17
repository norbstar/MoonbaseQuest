using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

using TMPro;

namespace Conways
{
    public class ConwaysPanelUIManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] GridLayoutGroup gridLayoutGroup;
        [SerializeField] Slider slider;
        public float Value { get { return slider.value; } set { slider.value = value; } }
        [SerializeField] public TextMeshProUGUI textUI;
        [SerializeField] PointerEventHandler eventHandler;
        [SerializeField] GameObject buttons;

        [Header("Prefabs")]
        [SerializeField] protected GameObject cellPrefab;

        [Header("Audio")]
        [SerializeField] protected AudioClip onHoverClip;

        [Header("Game")]
        [SerializeField] ConwaysGame game;

        [Header("Config")]
        [SerializeField] bool enableHaptics = false;
        public bool EnableHaptics { get { return enableHaptics; } }

        public enum GridSize
        {
            Small,
            Medium,
            Large
        }

        public delegate void OnModifyEvent(float value);
        public static event OnModifyEvent EventReceived;

        // Start is called before the first frame update
        void Start()
        {
            slider.onValueChanged.AddListener(delegate {
                OnValueChanged(slider.value);
            });

            for (int idx = 0; idx < buttons.transform.childCount; idx++)
            {
                var child = buttons.transform.GetChild(idx).gameObject;
                var button = child.GetComponent<Button>() as Button;

                button.onClick.AddListener(delegate {
                    OnClickButton(button);
                });
            }

            SetGridSize(slider.value);
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
                    OnPointerEnter(eventData);
                    break;

                case PointerEventHandler.Event.Exit:
                    OnPointerExit(eventData);
                    break;
            }
        }

        private void OnPointerEnter(PointerEventData eventData)
        {
            XRRayInteractor rayInteractor = null;
            
            if (TryGetXRRayInteractor(eventData.pointerId, out var interactor))
            {
                rayInteractor = interactor;
            }

            StartCoroutine(OnPointerEnterCoroutine(eventData, rayInteractor));
        }

        private IEnumerator OnPointerEnterCoroutine(PointerEventData eventData, XRRayInteractor rayInteractor)
        {
            var manager = slider.handleRect.gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;

            if (enableHaptics)
            {
                rayInteractor?.SendHapticImpulse(0.25f, 0.1f);
            }

            yield return manager.ScaleUp(1f, 1.1f);

            if (onHoverClip != null)
            {
                AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
            }

            yield return null;
        }

        private void OnPointerExit(PointerEventData eventData) => StartCoroutine(OnPointerExitCoroutine(eventData));

        private IEnumerator OnPointerExitCoroutine(PointerEventData eventData)
        {
            var manager = slider.handleRect.gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1.1f, 1f);
        }

        public void OnValueChanged(float value)
        {
            SetGridSize(value);
            EventReceived?.Invoke(slider.value);
        }

        private void SetGridSize(float value)
        {
            var size = (int) value;

            switch (size)
            {
                case (int) GridSize.Small:
                    textUI.text = "5x5";
                    AllocateCells(5);
                    break;

                case (int) GridSize.Medium:
                    textUI.text = "11x11";
                    AllocateCells(11);
                    break;

                case (int) GridSize.Large:
                    textUI.text = "21x21";
                    AllocateCells(21);
                    break;
            }
        }

        private void AllocateCells(int size)
        {
            Vector2 gridSize = gridLayoutGroup.GetComponent<RectTransform>().sizeDelta;
            gridLayoutGroup.cellSize = new Vector2(gridSize.x / size, gridSize.y / size);

            for (int itr = 0; itr < (gridLayoutGroup.transform.childCount); itr++)
            {
                Destroy(gridLayoutGroup.transform.GetChild(itr).gameObject);
            }

            for (int yItr = 0; yItr < size; yItr++)
            {
                for (int xItr = 0; xItr < size; xItr++)
                {
                    var instance = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, gridLayoutGroup.transform);
                    var manager = instance.GetComponent<ConwaysCellUIManager>() as ConwaysCellUIManager;
                    manager.Coord = new Vector2(xItr - (size / 2), yItr - (size / 2));
                }
            }
        }

        public void Reset()
        {
            for (int itr = 0; itr < game.transform.childCount; itr++)
            {
                Destroy(game.transform.GetChild(itr).gameObject);
            }
        }

        private void OnClickButton(Button button)
        {
            if (button.name.Equals("Clear Button"))
            {
                for (int itr = 0; itr < (gridLayoutGroup.transform.childCount); itr++)
                {
                    var cell = gridLayoutGroup.transform.GetChild(itr).gameObject;
                    var manager = cell.GetComponent<ConwaysCellUIManager>();
                    manager.IsSelected = false;
                }

                Reset();
            }
            else if (button.name.Equals("Execute Button"))
            {
                List<ConwaysCellUIManager> cells = new List<ConwaysCellUIManager>();

                for (int itr = 0; itr < (gridLayoutGroup.transform.childCount); itr++)
                {
                    var cell = gridLayoutGroup.transform.GetChild(itr).gameObject;
                    var manager = cell.GetComponent<ConwaysCellUIManager>();
                    
                    if (manager.IsSelected)
                    {
                        cells.Add(manager);
                    }
                }

                game.Init(cells);
            }
        }
    }
}