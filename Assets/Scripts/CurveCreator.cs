using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class CurveCreator : MonoBehaviour
{
    [SerializeField] int range = 100;
    [SerializeField] int steps = 20;

    [Header("Output")]
    [SerializeField] List<float> values;

    public IList<float> Values
    {
        get
        {
            if ((values != null) && (!values.Any()))
            {
                values = CreateValues();
            }

            return values;
        }
    }

    private List<float> CreateValues()
    {
        float b = (range / 5) / 3f;
        float c = Mathf.Log(16f);
        float stepSize = range / steps;
        float unitSize = 1f / steps;

        var values = new List<float>();

        for (int idx = 0; idx <= steps; idx++)
        {
            float value = b * (Mathf.Exp(c * (unitSize * idx)) - 1f);
            values.Add(value);
        }

        return values;
    }
}