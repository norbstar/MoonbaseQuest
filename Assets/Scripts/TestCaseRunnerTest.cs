using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestCaseRunnerTest : MonoBehaviour
{
    [SerializeField] List<string> expectedSequence;
    [SerializeField] List<string> postedSequence;

    private TestCaseRunner testCaseRunner;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
        StartCoroutine(PostSequenceCoroutine());
    }

    private void ResolveDependencies()
    {
        testCaseRunner = TestCaseRunner.GetInstance();
    }

    private IEnumerator PostSequenceCoroutine()
    {
        TestCaseRunner.EventReceived += OnEvent;
        
        testCaseRunner.SetExpectedSequence(expectedSequence);
        
        foreach (string item in postedSequence)
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
                    Debug.Log($"{Time.time} DataPoint[{idx}] Expected : {dataPoint.expected} Posted : {dataPoint.posted} Pass : {dataPoint.pass}");
                }
                
                break;

            case TestCaseRunner.State.End:
                Debug.Log($"{Time.time} End");
                break;
        }
    }
}