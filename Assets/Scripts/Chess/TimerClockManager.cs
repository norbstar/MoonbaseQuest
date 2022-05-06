using System;

namespace Chess
{
    public class TimerClockManager : ClockManager
    {
        public override void Awake()
        {
            base.Awake();
            Set(Minutes, Seconds);
        }

        protected override void OnUpdate(TimeSpan totalTicks)
        {
            TimeSpan remaining = new TimeSpan(duration.Ticks - TimeSpan.FromSeconds((totalTicks.Minutes * 60) + totalTicks.Seconds).Ticks);
            Set(remaining.Minutes, remaining.Seconds);
        }
    }
}