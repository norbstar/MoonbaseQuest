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

        public delegate void Event(Mode mode, PlayerMode lightPlayer, PlayerMode darkPlayer);
        public static event Event EventReceived;

        private ChessBoardManager chessBoardManager;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        void OnEnable() => NewGameUIManager.EventReceived += OnEvent;

        void OnDisable() => NewGameUIManager.EventReceived -= OnEvent;

        public void ShowAfterDelay(float seconds)
        {
            StartCoroutine(ShowAfterDelayCoroutine(seconds));
        }

        private IEnumerator ShowAfterDelayCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Show();
        }

        public void Show()
        {
            chessBoardManager.NewGameUI.SetActive(true);

            chessBoardManager.AttachLayerToControllers("UI Input Layer");
            uiManager.Show();
        }

        private void Hide()
        {
            chessBoardManager.NewGameUI.SetActive(false);

            uiManager.Hide();
            chessBoardManager.DetachLayerFromContollers("UI Input Layer");
        }
        
        public void OnEvent(NewGameManager.Mode mode)
        {
            Hide();

            switch (mode)
            {
                case NewGameManager.Mode.PVP:
                    EventReceived?.Invoke(mode, PlayerMode.Human, PlayerMode.Human);
                    break;

                case NewGameManager.Mode.PVB:
                    EventReceived?.Invoke(mode, PlayerMode.Human, PlayerMode.Bot);
                    break;

                case NewGameManager.Mode.BVB:
                    EventReceived?.Invoke(mode, PlayerMode.Bot, PlayerMode.Bot);
                    break;
            }   
        }
    }
}