using System;
using System.Collections;

using UnityEngine;

using TMPro;

public class StatsTerminalCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dateTextUI;
    [SerializeField] TextMeshProUGUI timeTextUI;
    [SerializeField] TextMeshProUGUI fpsTextUI;
    [SerializeField] float refreshInterval = 0.25f;

    private float lastInterval = 0;
    private int frames = 0;

    // Start is called before the first frame update
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;

        StartCoroutine(GenerateStats());
    }

    private IEnumerator GenerateStats()
    {
        while (isActiveAndEnabled)
        {
            var now = DateTime.Now;

            GenerateDate(now);
            GenerateTime(now);
            GenerateFPS();

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private void GenerateDate(DateTime now)
    {
        var date = now.ToString("dd/MM/yyyy");
        dateTextUI.text = date;
    }

    private void GenerateTime(DateTime now)
    {
        var time = now.ToString("hh:mm");
        timeTextUI.text = time;
    }

    private void GenerateFPS()
    {
        var fps = Convert.ToInt64(1.0f / Time.deltaTime);
        float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
        ms = (float) Math.Round(ms * 100f) / 100f;

        if (fps < 30)
            fpsTextUI.color = new Color(1f, 1f, 0f);
        else if (fps < 10)
            fpsTextUI.color = Color.red;
        else
            fpsTextUI.color = Color.green;

        fpsTextUI.text = $"{fps} [{ms}]";
    }

#if false
    private void RefreshFPS()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + refreshInterval)
        {
            float fps = frames / (timeNow - lastInterval);
            fps = (float) Math.Round(fps * 100f) / 100f;

            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
            ms = (float) Math.Round(ms * 100f) / 100f;

            fpsTextUI.text = $"{fps} [{ms}]";
            frames = 0;
            lastInterval = timeNow;
        }
    }
#endif
}