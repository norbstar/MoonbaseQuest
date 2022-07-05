// using System;
using System.Collections;
using System.Collections.Generic;
// using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// using TMPro;

namespace Chess
{
    public class SettingsPanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] GameObject panelBase;
        [SerializeField] ToggleGroupManager toggleGroupManager;
        // [SerializeField] TMP_Dropdown interactionModeDropdown;

        [Header("Config")]
        [SerializeField] float rotationSpeed = 5f;

        private Coroutine coroutine;
        private bool monitoring;

        // Start is called before the first frame update
        // IEnumerator Start()
        // {
            // interactionModeDropdown.onValueChanged.AddListener(delegate {
            //     ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);
            // });

            // float angle = 0;

            // angle = EulerAngle(15f) - panelBase.transform.localRotation.eulerAngles.y;
            // angle = EulerAngle(15f);
            // Debug.Log($"Angle : {angle}");
            // yield return coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));

            // angle = EulerAngle(-15f) - panelBase.transform.localRotation.eulerAngles.y;
            // angle = EulerAngle(-15f);
            // Debug.Log($"Angle : {angle}");
            // yield return coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
        // }

        void OnEnable()
        {
            toggleGroupManager.EventReceived += OnToogleGroupEvent;
            // interactionModeDropdown.value = (int) ((SettingsManager) SettingsManager.Instance).InteractionMode;
            toggleGroupManager.TurnOnByIndex((int) ((SettingsManager) SettingsManager.Instance).InteractionMode);

            if (panelBase != null)
            {
                panelBase.transform.eulerAngles = Vector3.zero;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (monitoring) return;

            if (Input.GetKeyDown(KeyCode.A))
            {
                float angle = EulerAngle(15f);
                coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                float angle = EulerAngle(-15f);
                coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
            }
        }

        void OnDisable()
        {
            toggleGroupManager.EventReceived -= OnToogleGroupEvent;
        }

        private void OnToogleGroupEvent(List<Toggle> group) { }

        public void OnPanelPointerEnter(BaseEventData eventData)
        {
            if (monitoring) return;
            
            var panel = ((PointerEventData) eventData).pointerEnter;
            // Debug.Log($"OnPanelPointerEnter Panel : {panel.name}");
            float angle = 0f;

            if (panel.name.Equals("Button Panel"))
            {
                // Rotate the panel base Y axis to 15 deg to draw focus
                // panelBase.transform.eulerAngles = new Vector3(0f, 15f, 0f);
                // angle = EulerAngle(15f - panelBase.transform.eulerAngles.y);
                angle = EulerAngle(15f);
            }
            else if (panel.name.Equals("Control Panel"))
            {
                // Rotate the panel base Y axis to -15 deg to draw focus
                // panelBase.transform.eulerAngles = new Vector3(0f, -15f, 0f);
                // angle = EulerAngle(-15f - panelBase.transform.eulerAngles.y);
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

            // var debugCanvas = GameObject.Find("Debug Canvas");
            // var manager = debugCanvas.GetComponent<DebugCanvasUIManager>() as DebugCanvasUIManager;
            // manager.textUI.text = $"Pending logging";

            LogIt($"Target Angle : {angle}", logging);
            LogIt($"Start Y : {panelBase.transform.eulerAngles.y}", logging);

            monitoring = true;

            Vector3 eulerAngles = panelBase.transform.eulerAngles;
            bool complete = false;

            do
            {
                eulerAngles += new Vector3(0f, (RelativeAngle(angle) < 0f) ? -1f : 1f, 0f) * Time.deltaTime * rotationSpeed;
                LogIt($"EulerAngles : {eulerAngles.y}", logging);

                if (RelativeAngle(angle) < 0f)
                {
                    complete = RelativeAngle(eulerAngles.y) < RelativeAngle(angle);
                    LogIt($"1 Complete : {complete}", logging);
                }
                else
                {
                    complete = RelativeAngle(eulerAngles.y) > RelativeAngle(angle);
                    LogIt($"2 Complete : {complete}", logging);
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
                LogIt($"Updated Y : {panelBase.transform.eulerAngles.y}", logging);

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
            else if (selectedButton.name.Equals("Confirm Button"))
            {
                // ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(interactionModeDropdown.value);

                int index = toggleGroupManager.Group.FindIndex(t => t.isOn);
                ((SettingsManager) SettingsManager.Instance).OnInteractionModeChange(index);
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }

        private void LogIt(string message, Logging logging = null) => LogItFunctions.LogIt("Settings", message, logging);
    }
}