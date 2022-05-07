using System;

namespace Chess
{
    public class TimerClockManager : ClockManager
    {
        public override void Awake()
        {
            UnityEngine.Debug.Log($"{UnityEngine.Time.time} Timer 1");
            base.Awake();
            Set(Minutes, Seconds);
        }

        protected override void OnUpdate(TimeSpan totalTicks)
        {
            long remainingTicks = duration.Ticks - TimeSpan.FromSeconds((totalTicks.Minutes * 60) + totalTicks.Seconds).Ticks;
            UnityEngine.Debug.Log($"{UnityEngine.Time.time} Timer 2 Duration : {duration} Total Ticks : {totalTicks.Ticks} Remaining Ticks : {remainingTicks}");
            TimeSpan remaining = new TimeSpan(remainingTicks);
            UnityEngine.Debug.Log($"{UnityEngine.Time.time} Timer 2 Duration : {duration} Total Ticks : {totalTicks.Ticks} Remaining Remaining : {remaining.Minutes} {remaining.Seconds}");
            Set((remaining.Hours * 60) + remaining.Minutes, remaining.Seconds);
        }

        public override void Reset()
        {
            base.Reset();
            Set(Minutes, Seconds);
        }
    }
}