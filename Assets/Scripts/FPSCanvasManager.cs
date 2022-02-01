using System;

using UnityEngine;

using TMPro;

public class FPSCanvasManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsTextUI;
    [SerializeField] float refreshInterval = 0.25f;

    private float refreshTime;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= refreshTime)
        {
            var fps = Convert.ToInt64(1.0f / Time.unscaledDeltaTime);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
            ms = (float) Math.Round(ms * 100f) / 100f;

            if (fps < 30)
                fpsTextUI.color = new Color(1f, 1f, 0f);
            else if (fps < 10)
                fpsTextUI.color = Color.red;
            else
                fpsTextUI.color = Color.green;

            fpsTextUI.text = $"{fps} [{ms}]";
            refreshTime += refreshInterval;
        }
    }
}