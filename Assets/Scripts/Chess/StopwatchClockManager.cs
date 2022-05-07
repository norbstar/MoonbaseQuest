using System;

namespace Chess
{
    public class StopwatchClockManager : ClockManager
    {
        public override void Awake()
        {
            UnityEngine.Debug.Log($"{UnityEngine.Time.time} Stopwatch 1");
            base.Awake();
            Set(0, 0);
        }

        protected override void OnUpdate(TimeSpan totalTicks) => Set(totalTicks.Minutes, totalTicks.Seconds);

        public override void Reset()
        {
            base.Reset();
            Set(0, 0);
        }
    }
}