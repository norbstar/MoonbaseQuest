using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(CurveCreator))]
public class CurveCreatorTest : MonoBehaviour
{
    private CurveCreator curveCreator;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        curveCreator = GetComponent<CurveCreator>() as CurveCreator;
    }

    // Start is called before the first frame update
    void Start()
    {
        IList<float> values = curveCreator.Values;

        foreach (float value in values)
        {
            Debug.Log($"Value : {value}");
        }
    }
}