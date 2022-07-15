using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Chess
{
    public class SettingsPanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] GameObject panelBase;
        [SerializeField] ToggleGroupManager interactionModeManager;
        [SerializeField] ToggleGroupManager playAsManager;
        [SerializeField] ToggleGroupManager playOrderManager;
        [SerializeField] ToggleGroupManager oppositionSkillLevelManager;
        [SerializeField] ButtonGroupManager numberManager;
        [SerializeField] ButtonGroupManager intentManager;
        [SerializeField] ButtonGroupManager explitivesManager;

        [Header("Config")]
        [SerializeField] float rotationSpeed = 5f;

        private Coroutine coroutine;
        private bool monitoring;

        public override void Start()
        {
            base.Start();

            if (onToggleSelectClip != null)
            {
                interactionModeManager.OnToggleClip = onToggleSelectClip;
                playAsManager.OnToggleClip = onToggleSelectClip;
                playOrderManager.OnToggleClip = onToggleSelectClip;
                oppositionSkillLevelManager.OnToggleClip = onToggleSelectClip;
            }

            if (onButtonSelectClip != null)
            {
                if (numberManager.OnSelectClip == null)
                {
                    numberManager.OnSelectClip = onButtonSelectClip;
                }

                if (intentManager.OnSelectClip != null)
                {
                    intentManager.OnSelectClip = onButtonSelectClip;
                }

                if (explitivesManager.OnSelectClip != null)
                {
                    explitivesManager.OnSelectClip = onButtonSelectClip;
                }
            }
        }

        void OnEnable()
        {
            interactionModeManager.EventReceived += OnInteractionModeEvent;
            playAsManager.EventReceived += OnPlayAsEvent;
            playOrderManager.EventReceived += OnPlayOrderEvent;
            oppositionSkillLevelManager.EventReceived += OnOppositionSkillLevelEvent;
            numberManager.EventReceived += OnNumberEvent;
            intentManager.EventReceived += OnIntentEvent;
            explitivesManager.EventReceived += OnIntentEvent;

            interactionModeManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);
            playAsManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Set);
            playOrderManager.SetByIndex((int) (((SettingsManager) SettingsManager.Instance).PlayFirst ? 0 : 1));
            oppositionSkillLevelManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Skill);
            
            if (panelBase != null)
            {
                panelBase.transform.eulerAngles = Vector3.zero;
            }
        }

        void OnDisable()
        {
            interactionModeManager.EventReceived -= OnInteractionModeEvent;
            playAsManager.EventReceived -= OnPlayAsEvent;
            playOrderManager.EventReceived -= OnPlayOrderEvent;
            oppositionSkillLevelManager.EventReceived -= OnOppositionSkillLevelEvent;
            numberManager.EventReceived -= OnNumberEvent;
            intentManager.EventReceived -= OnIntentEvent;
            explitivesManager.EventReceived -= OnIntentEvent;
        }

        private void OnInteractionModeEvent(List<Toggle> group) { }

        private void OnPlayAsEvent(List<Toggle> group) { }

        private void OnPlayOrderEvent(List<Toggle> group) { }

        private void OnOppositionSkillLevelEvent(List<Toggle> group) { }
        
        private void OnNumberEvent(List<UnityEngine.UI.Button> group) { }

        private void OnIntentEvent(List<UnityEngine.UI.Button> group) { }

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

            if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
            else if (selectedButton.name.Equals("Apply Button"))
            {
                int index;

                index = interactionModeManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(index);
                
                index = playAsManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnPlayAsChange(index);

                index = playOrderManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnPlayOrderFirstChange(index);

                index = oppositionSkillLevelManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnOppositionSkillLevelChange(index);
                
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}