using System;

namespace Chess
{
    public class TimerClockManager : ClockManager
    {
        // Update is called once per frame
        void Update()
        {
            if (!running) return;
            
            // DateTime time = DateTime.Now;;
            // int elapsedSecs = (int) (time - startTime).TotalSeconds;
            // Set(secs - elapsedSecs);

            // int elapsedSecs = (int) (time - lastTime).Seconds;

            // totalSecs += elapsedSecs;
            // lastTime = time;
            // Set(Seconds - totalSecs);

            // var elapsedTime = DateTime.Now - startTime;
            // Refresh(DateTime.Now - startTime);
        }
    }
}