using System;

using UnityEngine;

[RequireComponent(typeof(CanvasGraph))]
public class FPSCanvasGraphProvider : MonoBehaviour
{
    [SerializeField] float refreshInterval = 0.5f;
    [SerializeField] float threshold = 60f;

    private CanvasGraph canvasGraph;
    private float refreshTime;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        canvasGraph.XAxisLabel = "Time (once per second)";
        canvasGraph.YAxisLabel = "FPS";
        canvasGraph.AddThreshold(threshold);
    }

    private void ResolveDependencies()
    {
        canvasGraph = GetComponent<CanvasGraph>() as CanvasGraph;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= refreshTime)
        {
            var fps = Convert.ToInt64(1.0f / Time.unscaledDeltaTime);
            canvasGraph.PlotElement(Time.time * 10, fps);
            refreshTime += refreshInterval;
        }
    }
}