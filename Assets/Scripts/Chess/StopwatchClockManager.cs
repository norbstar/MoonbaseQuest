using System;

using UnityEngine;

namespace Chess
{
    public class StopwatchClockManager : ClockManager
    {
         public override void Awake()
        {
            base.Awake();
            Set(0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (!running | expired) return;

            // long totalTicks = ticks + ((DateTime.Now.Ticks - startTicks) - startSpanTicks);
            long totalTicks = DateTime.Now.Ticks - startTicks;
            // Debug.Log($"Stopwatch Total Ticks : {totalTicks}");

            // long ticksRemaining = duration.Ticks - totalTicks;
            // Debug.Log($"Stopwatch Ticks Remaining : {ticksRemaining}");

            var timespan = new TimeSpan(totalTicks);

            int minutes = timespan.Minutes;
            // Debug.Log($"Stopwatch Minutes : {minutes}");

            int seconds = timespan.Seconds;
            Debug.Log($"Stopwatch Seconds : {seconds}");

            Set(minutes, seconds);
            
            TimeSpan elapsed = TimeSpan.FromSeconds(timespan.TotalSeconds);

            if (elapsed.Ticks >= duration.Ticks)
            {
                expired = true;
                Debug.Log("Stopwatch has expired");
                return;
            }
        }
    }
}