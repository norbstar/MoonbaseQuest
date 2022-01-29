using System.Collections.Generic;

using UnityEngine;

namespace Test
{
    [CreateAssetMenu(menuName = "Test Case Runner Sequence")]
    public class TestCaseRunnerSequenceScriptable : ScriptableObject
    {
        [SerializeField] List<string> sequence;

        public List<string> Sequence { get { return sequence; } }

    }
}