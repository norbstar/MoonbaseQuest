using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestCaseRunnerTest : MonoBehaviour
{
    [SerializeField] TestCaseRunner testCaseRunner;
    [SerializeField] List<string> sequence;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PostSequenceCoroutine());
    }

    private IEnumerator PostSequenceCoroutine()
    {
        TestCaseRunner.EventReceived += OnEvent;
        
        foreach (string item in sequence)
        {
            yield return new WaitForSeconds(1f);
            testCaseRunner.Post(item);
        }
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
                    Debug.Log($"{Time.time} DataPoint[{idx}] Expected : {dataPoint.expected} Posted : {dataPoint.posted} Result : {dataPoint.result}");
                }
                
                break;

            case TestCaseRunner.State.End:
                Debug.Log($"{Time.time} End");
                break;
        }
    }
}