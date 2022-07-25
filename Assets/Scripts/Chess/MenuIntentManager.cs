using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ActuationHandler))]
    public class MenuIntentManager : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] protected CanvasUIManager canvasManager;

        private ActuationHandler actuationHandler;

        public virtual void Awake() => ResolveDependencies();

        private void ResolveDependencies() => actuationHandler = GetComponent<ActuationHandler>() as ActuationHandler;

        void OnEnable() => actuationHandler.IntentEventReceived += IntentEvent;

        void OnDisable() => actuationHandler.IntentEventReceived -= IntentEvent;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IntentEvent(ActuationHandler.Intent.ToggleOculusMenu);
            }
        }

        public void IntentEvent(ActuationHandler.Intent intent)
        {
            // Debug.Log($"IntentEvent");

            switch (intent)
            {
                case ActuationHandler.Intent.ToggleOculusMenu:
                    bool isShown = canvasManager.IsShown(CanvasUIManager.PanelType.Shortcuts);
                    // Debug.Log($"IntentEvent IsShown {isShown}");

                    if (isShown)
                    {
                        // Debug.Log($"IntentEvent HideShortcutPanel");
                        HideShortcutPanel();
                    }
                    else
                    {
                        // Debug.Log($"IntentEvent ShowShortcutPanel");
                        ShowShortcutPanel();
                    }
                    break;
            }
        }

        private void ShowShortcutPanel() => canvasManager.ShowPanel(CanvasUIManager.PanelType.Shortcuts);

        private void HideShortcutPanel() => canvasManager.ClosePanel();
    }
}