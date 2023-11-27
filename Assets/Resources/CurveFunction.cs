using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CurveFunction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected AnimationCurve curve;

    protected float? startSec = null;
    protected float elapsedTimeSec;

    protected ICurveFunctionInput input;

    public void Init(ICurveFunctionInput input) => this.input = input;

    public void Exec() => startSec = Time.time;

    public bool IsRunning { get { return startSec != null; }}

    public void Cancel() => startSec = null;

    public virtual float Get() => curve.Evaluate(Time.time - startSec.Value);

    public virtual float Get(float value) => curve.Evaluate(value);
}
