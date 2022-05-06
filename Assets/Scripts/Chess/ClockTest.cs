using System.Collections;

using UnityEngine;

namespace Chess
{
    public class ClockTest : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] StopwatchClockManager stopwatch;
        [SerializeField] TimerClockManager timer;

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            stopwatch?.Go();
            timer?.Go();
        }
    }
}