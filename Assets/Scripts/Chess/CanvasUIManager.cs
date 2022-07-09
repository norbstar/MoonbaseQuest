using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class CanvasUIManager : MonoBehaviour
    {
         [Header("Components")]
         [SerializeField] BasePanelUIManager launchPanel;
         [SerializeField] BasePanelUIManager mainPanel;
         [SerializeField] BasePanelUIManager gamePanel;
         [SerializeField] BasePanelUIManager settingsPanel;
         [SerializeField] BasePanelUIManager exitPanel;

        public enum PanelType
        {
            Launch,
            Main,
            Game,
            Settings,
            Exit
        }

        private List<BasePanelUIManager> panels;

        void Awake()
        {
            panels =  new List<BasePanelUIManager>();

            panels.Add(launchPanel);
            panels.Add(mainPanel);
            panels.Add(gamePanel);
            panels.Add(settingsPanel);
            panels.Add(exitPanel);
        }

        public void ShowPanel(PanelType type) => StartCoroutine(SwitchPanelCoroutine(type));

        private bool TryGetActiveChild(out GameObject gameObject)
        {
            // for (int itr = 0; itr < transform.childCount; itr++)
            // {
            //     var child = transform.GetChild(itr).gameObject;

            //     if (child.activeSelf)
            //     {
            //         gameObject = child;
            //         return true;
            //     }
            // }

            foreach (BasePanelUIManager panel in panels)
            {
                if (panel.gameObject.activeSelf)
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

            // var child = transform.GetChild((int) type).gameObject;
            // yield return ShowPanelCoroutine(child);

            var panel = panels[(int) type].gameObject;
            yield return ShowPanelCoroutine(panel);
        }

        private IEnumerator ShowPanelCoroutine(GameObject panel)
        {
            panel.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            panel.SetActive(true);

            var manager = panel.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleUp(0.9f, 1f);
        }

        private IEnumerator HidePanelCoroutine(GameObject panel)
        {
            var manager = panel.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1f, 0.9f);

            panel.SetActive(false);
        }
    }
}