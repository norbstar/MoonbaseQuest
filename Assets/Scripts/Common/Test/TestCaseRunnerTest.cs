using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Test
{
    public class TestCaseRunnerTest : MonoBehaviour
    {
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
                foreach (string item in postedSequence)
                {
                    yield return new WaitForSeconds(1f);
                    testCaseRunner.Post(item);
                }
            }
        }
    }
}