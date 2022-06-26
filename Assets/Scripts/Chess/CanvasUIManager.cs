using System.Collections;

using UnityEngine;

namespace Chess
{
    public class CanvasUIManager : MonoBehaviour
    {
         [Header("Prefabs")]
         [SerializeField] GameObject launchPanel;
         [SerializeField] GameObject mainPanel;
         [SerializeField] GameObject settingsPanel;
         [SerializeField] GameObject exitPanel;

        public enum PanelType
        {
            Launch,
            Main,
            Settings,
            Exit
        }

        public void ShowPanel(PanelType type) => StartCoroutine(SwitchPanelCoroutine(type));

        private bool TryGetActiveChild(out GameObject gameObject)
        {
            for (int itr = 0; itr < transform.childCount; itr++)
            {
                var child = transform.GetChild(itr).gameObject;

                if (child.activeSelf)
                {
                    gameObject = child;
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

            var child = transform.GetChild((int) type).gameObject;
            yield return ShowPanelCoroutine(child);
        }

        private IEnumerator ShowPanelCoroutine(GameObject child)
        {
            child.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            child.SetActive(true);

            var manager = child.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleUp(0.9f, 1f);
        }

        private IEnumerator HidePanelCoroutine(GameObject child)
        {
            var manager = child.GetComponent<ScaleFXManager>() as ScaleFXManager;
            yield return manager.ScaleDown(1f, 0.9f);

            child.SetActive(false);
        }
    }
}