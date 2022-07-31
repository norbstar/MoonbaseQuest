using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Chess
{
    public class InGamePanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] GameObject panelBase;
        [SerializeField] ToggleGroupManager interactionModeManager;

        [Header("Config")]
        [SerializeField] float rotationSpeed = 5f;

        private Coroutine coroutine;
        private bool monitoring;

        public void Start()
        {
            if (onToggleSelectClip != null)
            {
                interactionModeManager.OnToggleClip = onToggleSelectClip;
            }
        }

        void OnEnable()
        {
            interactionModeManager.EventReceived += OnInteractionModeEvent;

            interactionModeManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);
            
            if (panelBase != null)
            {
                panelBase.transform.eulerAngles = Vector3.zero;
            }
        }

        void OnDisable()
        {
            interactionModeManager.EventReceived -= OnInteractionModeEvent;
        }

        private void OnInteractionModeEvent(List<Toggle> group) { }

        private void OnPlayAsEvent(List<Toggle> group) { }

        private void OnPlayOrderEvent(List<Toggle> group) { }

        private void OnOppositionSkillLevelEvent(List<Toggle> group) { }
        
        public void OnPanelPointerEnter(BaseEventData eventData)
        {
            if (monitoring) return;
            
            var panel = ((PointerEventData) eventData).pointerEnter;
            float angle = 0f;

            if (panel.name.Equals("Button Panel"))
            {
                angle = EulerAngle(15f);
            }
            else if (panel.name.Equals("Control Panel"))
            {
                angle = EulerAngle(-15f);
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
            coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
        }

        private float EulerAngle(float angle)
        {
            if (angle < 0f)
            {
                angle = 360f + angle;
            }
            
            return angle;
        }

        private float RelativeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle = -360f + angle;
            }

            return angle;
        }

        private IEnumerator RotatePanelBaseCoroutine(float angle)
        {
            Logging logging = new Logging
            {
                logToConsole = false,
                logToFile = true
            };

            monitoring = true;

            Vector3 eulerAngles = panelBase.transform.eulerAngles;
            bool complete = false;

            do
            {
                eulerAngles += new Vector3(0f, (RelativeAngle(angle) < 0f) ? -1f : 1f, 0f) * Time.deltaTime * rotationSpeed;

                if (RelativeAngle(angle) < 0f)
                {
                    complete = RelativeAngle(eulerAngles.y) < RelativeAngle(angle);
                }
                else
                {
                    complete = RelativeAngle(eulerAngles.y) > RelativeAngle(angle);
                }

                if (complete)
                {
                    eulerAngles = new Vector3
                    {
                        x = panelBase.transform.eulerAngles.x,
                        y = angle,
                        z = panelBase.transform.eulerAngles.z
                    };
                }

                panelBase.transform.eulerAngles = eulerAngles;

                yield return null;
            } while (!complete);

            monitoring = false;
        }

        public override void OnClickButton()
        {
            base.OnClickButton();

            if (selectedButton.name.Equals("Back To Game Button"))
            {
                canvasManager.ClosePanel();
            }
            else if (selectedButton.name.Equals("Abandon Game Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}