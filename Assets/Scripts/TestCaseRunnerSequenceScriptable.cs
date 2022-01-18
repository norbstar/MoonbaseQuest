using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Test Case Runner Sequence")]
public class TestCaseRunnerSequenceScriptable : ScriptableObject
{
    [SerializeField] List<string> sequence;

    public List<string> Sequence { get { return sequence; } }
}