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
                rotateFX.Start();
                break;

            case TestCaseRunner.State.End:
                rotateFX.Stop();
                TestCaseRunner.Result result = (TestCaseRunner.Result) data;
                Debug.Log($"Test Completed Result {result}");
                break;
        }
    } 
}