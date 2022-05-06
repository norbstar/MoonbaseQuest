using System;

namespace Chess
{
    public class StopwatchClockManager : ClockManager
    {
         public override void Awake()
        {
            base.Awake();
            Set(0, 0);
        }

        protected override void OnUpdate(TimeSpan totalTicks) => Set(totalTicks.Minutes, totalTicks.Seconds);
    }
}