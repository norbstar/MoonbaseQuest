using System.Collections;
using System.Reflection;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    public class NewGameManager : MonoBehaviour
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public enum Mode
        {
            PVP,
            PVB,
            BVB
        }

        [Header("Components")]
        [SerializeField] NewGameUIManager uiManager;

        public delegate void Event(Mode mode);
        public static event Event EventReceived;

        private ChessBoardManager chessBoardManager;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        void OnEnable() => NewGameUIManager.EventReceived += OnEvent;

        void OnDisable() => NewGameUIManager.EventReceived -= OnEvent;

        public void ShowAfterDelay(float seconds)
        {
            StartCoroutine(ShowNewGameUIAfterDelayCoroutine(seconds));
        }

        private IEnumerator ShowNewGameUIAfterDelayCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Show();
        }

        public void Show()
        {
            chessBoardManager.AttachLayerToControllers("UI Input Layer");
            uiManager.Show();
        }

        private void HideNewGameUI()
        {
            uiManager.Hide();
            chessBoardManager.DetachLayerFromContollers("UI Input Layer");
        }
        
        public void OnEvent(NewGameManager.Mode mode)
        {
            uiManager.Hide();
            EventReceived?.Invoke(mode);
        }
    }
}