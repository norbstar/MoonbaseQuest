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
        [SerializeField] Set associatedSet;
        public Set AssociatedSet { get { return associatedSet; } }
        [SerializeField] int minutes;
        public int Minutes { get { return minutes; } }
        [SerializeField] int seconds;
        public int Seconds { get { return seconds; } }

        public bool IsRunning { get { return running; } }

        public delegate void OnExpiredEvent(ClockManager instance);
        public static event OnExpiredEvent OnExpiredEventReceived;

        protected bool running, expired;
        protected long ticks, startTurnTicks;
        protected int lastMinutes, lastSeconds;
        protected TimeSpan duration;

        public virtual void Awake()
        {
            duration = TimeSpan.FromSeconds((minutes * 60) + seconds);
        }

        public void Resume()
        {
            startTurnTicks = DateTime.Now.Ticks;
            running = true;
        }

        public void Pause()
        {
            running = false;
            ticks += (DateTime.Now.Ticks - startTurnTicks);
        }

        protected abstract void OnUpdate(TimeSpan totalTicks);

        // Update is called once per frame
        void Update()
        {
            if (!running || expired) return;

            long totalTicks = ticks + (DateTime.Now.Ticks - startTurnTicks);
            var timespan = new TimeSpan(totalTicks);

            OnUpdate(timespan);

            TimeSpan elapsed = TimeSpan.FromSeconds(timespan.TotalSeconds);

            if (elapsed.Ticks >= duration.Ticks)
            {
                expired = true;
                OnExpiredEventReceived?.Invoke(this);
            }
        }

        protected void Set(int minutes, int seconds)
        {
            string minsPadded = minutes.ToString("00");
            string secsPadded = seconds.ToString("00");
            textUI.text = $"{minsPadded}:{secsPadded}";
        }

        public virtual void Reset()
        {
            ticks = 0;
            running = expired = false;
        }
    }
}