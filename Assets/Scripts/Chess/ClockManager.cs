using System;

using UnityEngine;

using TMPro;

namespace Chess
{
    public abstract class ClockManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] TextMeshProUGUI textUI;

        public string Text
        {
            get
            {
                return textUI.text;
            }
            
            set
            {
                textUI.text = value;
            }
        }

        [Header("Config")]
        [SerializeField] int seconds;
        public int Seconds { get { return seconds; } }

        [SerializeField] bool startOnLoad;

        protected DateTime startTime;
        protected DateTime lastTime;
        protected DateTime trackedTime;
        protected int totalSecs;
        protected bool running;

        // Start is called before the first frame update
        void Start()
        {
            if (startOnLoad)
            {
                StartClock(seconds);
            }
        }

        public void StartClock(int seconds)
        {
            this.seconds = seconds;
            startTime = lastTime = trackedTime = DateTime.Now;
            ResetClock();
            running = true;
        }

        public void PauseClock() => running = false;

        public void StopClock() => running = false;

        public void ResetClock()
        {
            textUI.text = "00:00";
            totalSecs = 0;
        }

        protected void Refresh(DateTime now)
        {
            trackedTime += (now - lastTime);
            lastTime = now;
            int elapsedSecs = (int) trackedTime.TimeOfDay.TotalSeconds;
        }

        private void Set(int secs)
        {
            int mins = secs / 60;
            string minsPadded = mins.ToString("00");
            
            secs -= secs * mins;
            string secsPadded = secs.ToString("00");

            textUI.text = $"{minsPadded}:{secsPadded}";
        }
    }
}