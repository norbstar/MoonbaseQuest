using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(TestCaseRunner))]
[RequireComponent(typeof(FX.RotateFX))]
public class TestCaseRunnerTest : MonoBehaviour
{
    [SerializeField] List<string> sequence;

    private TestCaseRunner testCaseRunner;
    private FX.RotateFX rotateFX;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        testCaseRunner = GetComponent<TestCaseRunner>() as TestCaseRunner;
        rotateFX = GetComponent<FX.RotateFX>() as FX.RotateFX;
    }

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
                rotateFX.Start();
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
                rotateFX.Stop();
                break;
        }
    }
}