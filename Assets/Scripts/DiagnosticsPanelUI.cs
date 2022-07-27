using System;

using UnityEngine;

using TMPro;

public class DiagnosticsPanelUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUIA;
    [SerializeField] TextMeshProUGUI textUIB;
    
    // Update is called once per frame
    void Update()
    {
        var elapsedTime = Time.timeSinceLevelLoad;
        var ts = TimeSpan.FromSeconds(elapsedTime);

        textUIA.text = $"{ts.Hours.ToString("D2")}:{ts.Minutes.ToString("D2")}:{ts.Seconds.ToString("D2")}:{((int) ts.Milliseconds / 10).ToString("D2")}";

        elapsedTime = Time.time;
        ts = TimeSpan.FromSeconds(elapsedTime);

        textUIB.text = $"{ts.Hours.ToString("D2")}:{ts.Minutes.ToString("D2")}:{ts.Seconds.ToString("D2")}:{((int) ts.Milliseconds / 10).ToString("D2")}";
    }
}