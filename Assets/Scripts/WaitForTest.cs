using System.Collections;

using UnityEngine;

// Demonstrates the use of the WaitUntil enumerator operation to wait on a condition (block)
// before being permitted to continue. Useful for circumstances where you can not
// yield return StartCoroutine(...) where the entry point does not return an IEnumerator.
public class WaitForTest : MonoBehaviour
{
    private bool isComplete;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunTestCoroutine());
    }

    [ContextMenu("Run Basic Test")]
    private void RunTest()
    {
        for (int itr = 0; itr < 10; itr++)
        {
            Debug.Log($"Itr : {itr}");
        }
    }

    private IEnumerator RunTestCoroutine()
    {
        Debug.Log("Start test");
        StartCoroutine(RunToCompletionCoroutine());
        Debug.Log("Waiting until coroutine flags the complete condition before continuing");
        yield return new WaitUntil(() => isComplete);
        Debug.Log("End test");
    }

    private IEnumerator RunToCompletionCoroutine()
    {
        isComplete = false;
        int itr = 0;

        while (!isComplete)
        {
            isComplete = (++itr == 10);
            Debug.Log($"Itr : {itr} Is Complete {isComplete}");

            yield return null;
        }

        Debug.Log($"Coroutine ran to completion");
    }
}
