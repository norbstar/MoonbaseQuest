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
        // [SerializeField] TMP_Dropdown interactionModeDropdown;

        [Header("Config")]
        [SerializeField] float rotationSpeed = 5f;

        private Coroutine coroutine;
        private bool monitoring;
        // private GameObject applyButton;

        // void Awake()
        // {
        //     applyButton = buttons.transform.GetChild(1).gameObject;
        // }

        // Start is called before the first frame update
        // IEnumerator Start()
        // {
            // interactionModeDropdown.onValueChanged.AddListener(delegate {
            //     ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);
            // });
        // }

        void OnEnable()
        {
            interactionModeManager.EventReceived += OnInteractionModeEvent;
            playAsManager.EventReceived += OnPlayAsEvent;
            playOrderManager.EventReceived += OnPlayOrderEvent;
            oppositionSkillLevelManager.EventReceived += OnOppositionSkillLevelEvent;

            // interactionModeDropdown.value = (int) ((SettingsManager) SettingsManager.Instance).InteractionMode;
            interactionModeManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);
            playAsManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Set);
            playOrderManager.SetByIndex((int) (((SettingsManager) SettingsManager.Instance).PlayFirst ? 0 : 1));
            oppositionSkillLevelManager.SetByIndex((int) ((SettingsManager) SettingsManager.Instance).Skill);
            
            if (panelBase != null)
            {
                panelBase.transform.eulerAngles = Vector3.zero;
            }
        }

         // Update is called once per frame
        // void Update()
        // {
        //     if (monitoring) return;

        //     else if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         float angle = EulerAngle(15f);
        //         coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
        //     }
        //     else if (Input.GetKeyDown(KeyCode.D))
        //     {
        //         float angle = EulerAngle(-15f);
        //         coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
        //     }
        // }

        void OnDisable()
        {
            interactionModeManager.EventReceived -= OnInteractionModeEvent;
            playAsManager.EventReceived -= OnPlayAsEvent;
            playOrderManager.EventReceived -= OnPlayOrderEvent;
            oppositionSkillLevelManager.EventReceived -= OnOppositionSkillLevelEvent;
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

            if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
            else if (selectedButton.name.Equals("Apply Button"))
            {
                // ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);

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