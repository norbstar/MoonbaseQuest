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
        if (testCaseRunner != null)
        {
            testCaseRunner.SetExpectedSequence(expectedSequence);
            
            foreach (string item in postedSequence)
            {
                yield return new WaitForSeconds(1f);
                testCaseRunner.Post(item);
            }
        }
    }
}