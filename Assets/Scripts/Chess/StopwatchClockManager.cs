using System;

namespace Chess
{
    public class StopwatchClockManager : ClockManager
    {
        // Update is called once per frame
        void Update()
        {
            if (!running) return;

            // DateTime time = DateTime.Now;
            // int elapsedSecs = (int) (time - startTime).TotalSeconds;
            // Set(elapsedSecs);
            
            // int elapsedSecs = (int) (time - lastTime).Seconds;

            // totalSecs += elapsedSecs;
            // lastTime = time;
            // Set(totalSecs);

            Refresh(DateTime.Now);
        }
    }
}