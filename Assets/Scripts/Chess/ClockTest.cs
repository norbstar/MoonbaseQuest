using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class ClockTest : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<StopwatchClockManager> stopwatchss;
        [SerializeField] List<TimerClockManager> timers;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (StopwatchClockManager stopwatch in stopwatchss)
                {
                    if (stopwatch.IsRunning)
                    {
                        stopwatch.Pause();
                    }
                    else
                    {
                        stopwatch.Resume();
                    }
                }

                foreach (TimerClockManager timer in timers)
                {
                    if (timer.IsRunning)
                    {
                        timer.Pause();
                    }
                    else
                    {
                        timer.Resume();
                    }
                }
            }
        }
    }
}