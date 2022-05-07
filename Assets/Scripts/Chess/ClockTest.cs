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
                UnityEngine.Debug.Log($"{Time.time} Test 1");
                foreach (StopwatchClockManager stopwatch in stopwatchss)
                {
                    if (stopwatch.IsRunning)
                    {
                        UnityEngine.Debug.Log($"{Time.time} Test 2");
                        stopwatch.Pause();
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"{Time.time} Test 3");
                        stopwatch.Run();
                    }
                }

                foreach (TimerClockManager timer in timers)
                {
                    if (timer.IsRunning)
                    {
                        UnityEngine.Debug.Log($"{Time.time} Test 4");
                        timer.Pause();
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"{Time.time} Test 5");
                        timer.Run();
                    }
                }
            }
        }
    }
}