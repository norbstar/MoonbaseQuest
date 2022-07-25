using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class CanvasUIManager : MonoBehaviour
    {
         [Header("Components")]
         [SerializeField] LaunchPanelUIManager launchPanel;
         [SerializeField] MainPanelUIManager mainPanel;
         [SerializeField] GamePanelUIManager gamePanel;
         [SerializeField] InGamePanelUIManager inGamePanel;
         [SerializeField] SettingsPanelUIManager settingsPanel;
         [SerializeField] ExitPanelUIManager exitPanel;
         [SerializeField] ShortcutPanelUIManager shortcutPanel;

        // [Header("Timings")]
        // [SerializeField] float initialDelaySec = 2.5f;
        // [SerializeField] float fadeInDurationSec = 0.5f;
    
        public enum PanelType
        {
            Launch,
            Main,
            Game,
            InGame,
            Settings,
            Exit,
            Shortcuts
        }

        private List<BasePanelUIManager> panels;
        private GameObject activePanel;
        // private CanvasGroup canvasGroup;

        void Awake()
        {
            // ResolveDependencies();
            
            panels =  new List<BasePanelUIManager>();

            panels.Add(launchPanel);
            panels.Add(mainPanel);
            panels.Add(gamePanel);
            panels.Add(inGamePanel);
            panels.Add(settingsPanel);
            panels.Add(exitPanel);
            panels.Add(shortcutPanel);
        }

        // private void ResolveDependencies() => canvasGroup = GetComponent<CanvasGroup>() as CanvasGroup;

        // Start is called before the first frame update
        // IEnumerator Start()
        // {
        //     yield return StartCoroutine(ShowPanelCoroutine(PanelType.Shortcuts, 3.5f));

        //     if (canvasGroup != null)
        //     {
        //         yield return StartCoroutine(FadeInCanvasGroupCoroutine());
        //     }
        // }

        // void Start() => ShowPanel(PanelType.Shortcuts);

        // private IEnumerator ShowPanelCoroutine(PanelType type, float delay = 0f)
        // {
        //     yield return new WaitForSeconds(delay);
        //     ShowPanel(type);
        // }

        // private IEnumerator FadeInCanvasGroupCoroutine()
        // {
        //     yield return new WaitForSeconds(initialDelaySec);

        //     bool complete = false;
            
        //     float startTime = Time.time;
        //     // Debug.Log($"Start {startTime}");
            
        //     float endTime = startTime + fadeInDurationSec;
        //     // Debug.Log($"End {endTime}");

        //     while (!complete)
        //     {
        //         float fractionComplete = (Time.time - startTime) / fadeInDurationSec;
        //         // Debug.Log($"{Time.time} FC {fractionComplete}");
        //         complete = (fractionComplete >= 1.0f);
                
        //         canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, fractionComplete);    
                
        //         yield return null;
        //     }
        // }

        public void ShowPanel(PanelType type) => StartCoroutine(SwitchPanelCoroutine(type));

        public void ClosePanel() => StartCoroutine(ClosePanelCoroutine());

        public bool IsShown(PanelType type)
        {
            bool isShown = false;

            if (activePanel != null)
            {
                switch (type)
                {
                    case PanelType.Launch:
                        isShown = GameObject.ReferenceEquals(activePanel, launchPanel.gameObject);
                        break;

                    case PanelType.Main:
                        isShown = GameObject.ReferenceEquals(activePanel, mainPanel.gameObject);
                        break;

                    case PanelType.Game:
                        isShown = GameObject.ReferenceEquals(activePanel, gamePanel.gameObject);
                        break;

                    case PanelType.InGame:
                        isShown = GameObject.ReferenceEquals(activePanel, inGamePanel.gameObject);
                        break;

                    case PanelType.Settings:
                        isShown = GameObject.ReferenceEquals(activePanel, settingsPanel.gameObject);
                        break;

                    case PanelType.Exit:
                        isShown = GameObject.ReferenceEquals(activePanel, exitPanel.gameObject);
                        break;

                    case PanelType.Shortcuts:
                        isShown = GameObject.ReferenceEquals(activePanel, shortcutPanel.gameObject);
                        break;
                }
            }

            return isShown;
        }

        private bool TryGetActiveChild(out GameObject gameObject)
        {
            foreach (BasePanelUIManager panel in panels)
            {
                if (panel != null && panel.gameObject.activeSelf)
                {
                    gameObject = panel.gameObject;
                    return true;
                }
            }

            gameObject = default(GameObject);
            return false;
        }

        private IEnumerator SwitchPanelCoroutine(PanelType type)
        {
            if (TryGetActiveChild(out GameObject activeChild))
            {
                activeChild.GetComponent<BasePanelUIManager>().ResetButtons();
                yield return HidePanelCoroutine(activeChild);
            }

            var panel = panels[(int) type].gameObject;
            yield return ShowPanelCoroutine(panel);
        }

        private IEnumerator ClosePanelCoroutine()
        {
            if (TryGetActiveChild(out GameObject activeChild))
            {
                activeChild.GetComponent<BasePanelUIManager>().ResetButtons();
                yield return HidePanelCoroutine(activeChild);
            }
        }

        private IEnumerator ShowPanelCoroutine(GameObject panel)
        {
            panel.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            panel.SetActive(true);
            
            activePanel = panel;

            var manager = panel.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleUp(0.9f, 1f);
            
        }

        private IEnumerator HidePanelCoroutine(GameObject panel)
        {
            var manager = panel.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1f, 0.9f);

            panel.SetActive(false);

            activePanel = null;
        }
    }
}