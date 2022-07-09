using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Chess
{
    public class TriPanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] GameObject panelBase;

        [Header("Config")]
        [SerializeField] float rotationSpeed = 5f;

        private Coroutine coroutine;
        private bool monitoring;

        void OnEnable()
        {
            if (panelBase != null)
            {
                panelBase.transform.eulerAngles = Vector3.zero;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (monitoring) return;

            if (Input.GetKeyDown(KeyCode.S))
            {
                float angle = EulerAngle(0f);
                coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
            }
            else if (Input.GetKeyDown(KeyCode.A))
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

        public void OnPanelPointerEnter(BaseEventData eventData)
        {
            if (monitoring) return;
            
            var panel = ((PointerEventData) eventData).pointerEnter;
            float angle = 0f;

            if (panel.name.Equals("Button Panel"))
            {
                angle = EulerAngle(15f);
            }
            else if (panel.name.Equals("Main Panel"))
            {
                angle = EulerAngle(0f);
            }
            else if (panel.name.Equals("Content Panel"))
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
                logToConsole = true,
                logToFile = false
            };

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
                else if (RelativeAngle(angle) > 0f)
                {
                    complete = RelativeAngle(eulerAngles.y) > RelativeAngle(angle);
                    LogIt($"2 Complete : {complete}", logging);
                }
                // else if (RelativeAngle(angle) == 0f)
                // {
                //     complete = true;
                //     LogIt($"3 Complete : {complete}", logging);
                // }

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
        }

        private void LogIt(string message, Logging logging = null) => LogItFunctions.LogIt("Settings", message, logging);
    }
}