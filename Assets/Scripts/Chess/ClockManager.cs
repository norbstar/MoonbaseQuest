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

        public bool IsRunning { get { return running; } }

        protected bool running, expired;
        protected long ticks, startTicks, startSpanTicks;
        protected int lastMinutes, lastSeconds;
        protected TimeSpan duration;

        public virtual void Awake()
        {
            UnityEngine.Debug.Log($"{Time.time} Clock 1");
            duration = TimeSpan.FromSeconds((minutes * 60) + seconds);
        }

        public void Run()
        {
            UnityEngine.Debug.Log($"{Time.time} Clock 2");
            if (startTicks == -1)
            {
                startTicks = DateTime.Now.Ticks;
            }

            startSpanTicks = SpanTicks;
            running = true;
        }

        public void Pause()
        {
            UnityEngine.Debug.Log($"{Time.time} Clock 3");
            running = false;
            ticks += (DateTime.Now.Ticks - startSpanTicks);
        }

        protected abstract void OnUpdate(TimeSpan totalTicks);

        // Update is called once per frame
        void Update()
        {
            if (!running || expired) return;
            UnityEngine.Debug.Log($"{Time.time} Clock 4 Running : {running} Expired : {expired}");

            long totalTicks = ticks + ((DateTime.Now.Ticks - startTicks) - startSpanTicks);
            var timespan = new TimeSpan(totalTicks);

            OnUpdate(timespan);

            TimeSpan elapsed = TimeSpan.FromSeconds(timespan.TotalSeconds);

            if (elapsed.Ticks >= duration.Ticks)
            {
                expired = true;
            }
        }

        protected long SpanTicks { get { return DateTime.Now.Ticks - startTicks; } }

        protected void Set(int minutes, int seconds)
        {
            UnityEngine.Debug.Log($"{Time.time} Clock 5");
            string minsPadded = minutes.ToString("00");
            string secsPadded = seconds.ToString("00");
            textUI.text = $"{minsPadded}:{secsPadded}";
        }

        public virtual void Reset()
        {
            ticks = 0;
            startTicks = -1L;
            running = expired = false;
        }
    }
}