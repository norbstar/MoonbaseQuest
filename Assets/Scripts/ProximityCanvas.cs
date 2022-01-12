using System;

using UnityEngine;

using TMPro;

public class ProximityCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;

    public void SetProximity(float proximity)
    {
        textUI.text = RoundDown(proximity, 2).ToString();
    }

    private double RoundDown(double value, int decimalPlaces)
    {
        return Math.Floor(value * Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces);
    }
}