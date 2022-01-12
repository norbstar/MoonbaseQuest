using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AccelerationTest : MonoBehaviour
{
    [SerializeField] int range = 500;
    [SerializeField] int steps = 10;
    [SerializeField] GameObject prefab;
    [SerializeField] Image panel;

    private float value;
    private IList<float> values;

    // Start is called before the first frame update
    void Start()
    {
        float b = (range / 5) / 3f;
        float c = Mathf.Log(16f);
        float stepSize = range / steps;
        float unitSize = 1f / steps;

        values = new List<float>();

        for (int idx = 0; idx <= steps; idx++)
        {
            float value = b * (Mathf.Exp(c * (unitSize * idx)) - 1f);
            PlotPoint(idx * stepSize, value);
            // Debug.Log($"Value : {value}");
            values.Add(value);

            // var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            // instance.transform.parent = panel.transform;
            // instance.transform.localPosition = new Vector3(idx * stepSize, value, 0f);
            // instance.transform.localScale *= 0.25f;
        }

        foreach (float value in values)
        {
            Debug.Log($"Value : {value}");
        }

        // float value;
        
        // value = b * (Mathf.Exp(c * (unitSize * 0)) - 1f);
        // PlotPoint(0 * stepSize, value);
        // Debug.Log($"Value : {value}");

        // value = b * (Mathf.Exp(c * (unitSize * (steps / 2))) - 1f);
        // PlotPoint((steps / 2) * stepSize, value);
        // Debug.Log($"Value : {value}");

        // value = b * (Mathf.Exp(c * (unitSize * steps)) - 1f);
        // PlotPoint(steps * stepSize, value);
        // Debug.Log($"Value : {value}");

        // value = b * (Mathf.Exp(c * 0f) - 1f);
        // Debug.Log($"Y : {0f} Value : {value}");

        // value = b * (Mathf.Exp(c * 0.5f) - 1f);
        // Debug.Log($"Y : {0.5f} Value : {value}");

        // value = b * (Mathf.Exp(c * 1f) - 1f);
        // Debug.Log($"Y : {1f} Value : {value}");
    }

    private void PlotPoint(float x, float y)
    {
        var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = panel.transform;
        instance.transform.localPosition = new Vector3(x, y, 0f);
        instance.transform.localScale *= 0.25f;       
    }
}