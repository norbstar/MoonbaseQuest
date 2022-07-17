using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess
{
    public class GamePanelUIManager : BasePanelUIManager
    {
        [Header("Custom Components")]
        [SerializeField] ScenePanelUIManager scenePanel;
        [SerializeField] GameObject panelBase;
        [SerializeField] NewPanelUIManager newPanel;
        [SerializeField] SavedPanelUIManager savedPanel;

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
            else if (panel.name.Equals("New Panel") || panel.name.Equals("Saved Panel"))
            {
                angle = EulerAngle(-15f);
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
            coroutine = StartCoroutine(RotatePanelBaseCoroutine(angle));
        }

        public ScenePanelUIManager ScenePanel { get { return scenePanel; } }

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

            if (selectedButton.name.Equals("New Button"))
            {
                savedPanel.gameObject.SetActive(false);
                newPanel.gameObject.SetActive(true);
            }
            else if (selectedButton.name.Equals("Saved Button"))
            {
                newPanel.gameObject.SetActive(false);
                savedPanel.gameObject.SetActive(true);
            }
            else if (selectedButton.name.Equals("Back Button"))
            {
                canvasManager.ShowPanel(CanvasUIManager.PanelType.Main);
            }
        }
    }
}