using System;

using UnityEngine;

namespace Chess
{
    public class TimerClockManager : ClockManager
    {
        public override void Awake()
        {
            base.Awake();
            Set(Minutes, Seconds);
        }

        // Update is called once per frame
        void Update()
        {
            if (!running | expired) return;

            // long totalTicks = ticks + ((DateTime.Now.Ticks - startTicks) - startSpanTicks);
            long totalTicks = DateTime.Now.Ticks - startTicks;
            // Debug.Log($"Timer Total Ticks : {totalTicks}");

            // long ticksRemaining = duration.Ticks - totalTicks;
            // Debug.Log($"Timer Ticks Remaining : {ticksRemaining}");

            // var timespan = new TimeSpan(ticksRemaining);

            // int minutesRemaining = timespan.Minutes;
            // Debug.Log($"Minutes Remaining : {minutesRemaining}");

            // int secondsRemaining = timespan.Seconds;
            // Debug.Log($"Seconds Remaining : {secondsRemaining}");

            var timespan = new TimeSpan(totalTicks);

            int minutes = timespan.Minutes;
            // Debug.Log($"Minutes : {minutes}");

            int seconds = timespan.Seconds;
            Debug.Log($"Timer Seconds : {seconds}");

            TimeSpan remaining = new TimeSpan(duration.Ticks - TimeSpan.FromSeconds((minutes * 60) + seconds).Ticks);

            int minutesRemaining = remaining.Minutes;/*Minutes - minutes;*/
            // Debug.Log($"Timer Minutes Remaining : {minutesRemaining}");

            int secondsRemaining = remaining.Seconds;/*Seconds - seconds;*/
            Debug.Log($"Timer Seconds Remaining : {secondsRemaining}");

            Set(minutesRemaining, secondsRemaining);

            TimeSpan elapsed = TimeSpan.FromSeconds(timespan.TotalSeconds);

            if (elapsed.Ticks >= duration.Ticks)
            {
                expired = true;
                Debug.Log("Timer has expired");
                return;
            }
        }
    }
}