using System.Collections.Generic;

using UnityEngine;

public class TestCaseRunnerSequence : MonoBehaviour
{
    [SerializeField] List<string> sequence;

    public List<string> Sequence { get { return sequence; } }
}