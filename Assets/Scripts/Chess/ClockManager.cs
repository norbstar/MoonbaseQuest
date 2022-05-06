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
        [SerializeField] int minutes;
        public int Minutes { get { return minutes; } }
        [SerializeField] int seconds;
        public int Seconds { get { return seconds; } }

        protected bool running, expired;
        protected long ticks, startTicks, startSpanTicks;
        protected int lastMinutes, lastSeconds;
        protected TimeSpan duration;

        public virtual void Awake() => duration = TimeSpan.FromSeconds((minutes * 60) + seconds);

        public void Go()
        {
            startTicks = DateTime.Now.Ticks;
            Resume();
        }

        public void Resume()
        {
            startSpanTicks = SpanTicks;
            running = true;
        }

        public void Pause()
        {
            running = false;
            ticks += (DateTime.Now.Ticks - startSpanTicks);
        }

        protected long SpanTicks { get { return DateTime.Now.Ticks - startTicks; } }

        protected void Set(int minutes, int seconds)
        {
            string minsPadded = minutes.ToString("00");
            string secsPadded = seconds.ToString("00");
            textUI.text = $"{minsPadded}:{secsPadded}";

            UnityEngine.Debug.Log($"{textUI.text}");
        }

        protected void Reset()
        {
            ticks = 0;
            running = expired = false;
        }
    }
}