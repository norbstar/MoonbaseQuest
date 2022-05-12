using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    public class ClocksManager : MonoBehaviour
    {
        [Header("Clocks")]
        [SerializeField] TimerClockManager lightClock;
        [SerializeField] TimerClockManager darkClock;
        
        private ChessBoardManager chessBoardManager;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        public void ResetClocks()
        {
            lightClock.Reset();
            darkClock.Reset();
        }

        public void PauseClocks()
        {
            lightClock.Pause();
            darkClock.Pause();
        }

        public void PauseActiveClock()
        {
            switch (chessBoardManager.ActiveSet)
            {
                case Set.Light:
                    lightClock.Pause();
                    break;

                case Set.Dark:
                    darkClock.Pause();
                    break;
            }
        }

        public void ResumeActiveClock()
        {
            switch (chessBoardManager.ActiveSet)
            {
                case Set.Light:
                    lightClock.Resume();
                    break;

                case Set.Dark:
                    darkClock.Resume();
                    break;
            }
        }
    }
}