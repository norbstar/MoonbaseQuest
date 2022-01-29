using System.Collections.Generic;

using UnityEngine;

namespace Test
{
    public class TestCaseRunnerEventReceiver : MonoBehaviour
    {
        void OnEnable()
        {
            TestCaseRunner.EventReceived += OnEvent;
        }

        void OnDisable()
        {
            TestCaseRunner.EventReceived += OnEvent;
        }

        public void OnEvent(TestCaseRunner.State state, object data)
        {
            switch (state)
            {
                case TestCaseRunner.State.Start:
                    Debug.Log($"{Time.time} Start");
                    break;

                case TestCaseRunner.State.Pass:
                    Debug.Log($"{Time.time} Pass");
                    break;

                case TestCaseRunner.State.Fail:
                    Debug.Log($"{Time.time} Fail");

                    List<TestCaseRunner.DataPoint> dataPoints = (List<TestCaseRunner.DataPoint>) data;
                    
                    for (int idx = 0; idx < dataPoints.Count; idx++)
                    {
                        TestCaseRunner.DataPoint dataPoint = dataPoints[idx];
                        Debug.Log($"{Time.time} DataPoint[{idx}] Expected : {dataPoint.expected} Posted : {dataPoint.posted} Pass : {dataPoint.pass}");
                    }
                    
                    break;

                case TestCaseRunner.State.End:
                    Debug.Log($"{Time.time} End");
                    break;
            }
        }
    }
}