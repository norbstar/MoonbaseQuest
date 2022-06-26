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

        // private GameObject activeChild;

        // Start is called before the first frame update
        // void Start() => ShowPanel(PanelType.Launch);

        public void ShowPanel(PanelType type)
        {
        //     if (transform.childCount > 0)
        //     {
        //         var child = transform.GetChild(0).gameObject;
        //         Destroy(child);
        //     }

        //     GameObject instance = null;

        //     switch(type)
        //     {
        //         case PanelType.Launch:
        //             instance = Instantiate(launchPanel);
        //             break;
                
        //         case PanelType.Main:
        //             instance = Instantiate(mainPanel);
        //             break;

        //         case PanelType.Settings:
        //             instance = Instantiate(settingsPanel);
        //             break;

        //         case PanelType.Exit:
        //             instance = Instantiate(exitPanel);
        //             break;
        //     }

        //     instance.transform.parent = transform;
        //     instance.transform.localPosition = Vector3.zero;

            // GameObject child = null;

            // for (int itr = 0; itr < transform.childCount; itr++)
            // {
            //     child = transform.GetChild(itr).gameObject;
            //     child.GetComponent<BasePanelUIManager>().ResetButtons();
            //     child.SetActive(false);
            // }

            StartCoroutine(SwitchPanelCoroutine(type));

            // child = transform.GetChild((int) type).gameObject;
            // child.SetActive(true);

            // activeChild = child;
        }

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

            // if (activeChild != null)
            // {
            //     activeChild.GetComponent<BasePanelUIManager>().ResetButtons();
            //     yield return HidePanelCoroutine(activeChild);
            // }

            var child = transform.GetChild((int) type).gameObject;
            yield return ShowPanelCoroutine(child);

            // activeChild = child;
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