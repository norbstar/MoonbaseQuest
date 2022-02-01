using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CanvasGraph : MonoBehaviour
{
    [SerializeField] private RectTransform container;

    [Header("Axis")]
    [SerializeField] TextMeshProUGUI xTextUI;
    [SerializeField] TextMeshProUGUI yTextUI;

    [Header("Demarkations")]
    [SerializeField] Color thresholdColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] float thresholdWidth = 1f;

    [Header("Points")]
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Vector2 pointSize = Vector2.one * 5;
    [SerializeField] private Color pointColor = Color.white;
    [SerializeField] private float maxY = 100f;

    [Header("Connections")]
    [SerializeField] private Color connectionColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float connectionWidth = 1f;

    public string XAxisLabel { get { return xTextUI.text; } set { xTextUI.text = value; } }
    public string YAxisLabel { get { return yTextUI.text; } set { yTextUI.text = value; } }

    private GameObject lastPoint = null;

    // Start is called before the first frame update
    // void Start()
    // {
    //     List<float> dataPoints = new List<float> { 0f, 22f, 37f, 45f, 43f, 32f, 34f, 43f, 64f, 85f, 100f };
    //     PlotGraph(dataPoints);
    // }

    public void AddThreshold(float threshold)
    {
        var gameObject = new GameObject("Threshold", typeof(Image));
        gameObject.transform.SetParent(container, false);

        var image = gameObject.GetComponent<Image>();
        image.color = thresholdColor;

        float graphWidth = container.sizeDelta.x;
        float graphHeight = container.sizeDelta.y;
        float y = (threshold / maxY) * graphHeight;

        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphWidth * 0.5f, y);
        rectTransform.sizeDelta = new Vector2(graphWidth, thresholdWidth);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
    }

    public GameObject PlotElement(float x, float y)
    {
        float graphHeight = container.sizeDelta.y;
        y = (y / maxY) * graphHeight;

        var point = PlotPoint(new Vector2(x, y));

        if (lastPoint != null)
        {
            ConnectPoints(lastPoint.GetComponent<RectTransform>().anchoredPosition, point.GetComponent<RectTransform>().anchoredPosition);
        }

        lastPoint = point;

        return point;
    }

    private GameObject PlotPoint(Vector2 anchoredPosition)
    {
        var gameObject = new GameObject("Point", typeof(Image));
        gameObject.transform.SetParent(container, false);
        
        var image = gameObject.GetComponent<Image>();
        image.sprite = pointSprite;
        image.color = pointColor;

        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = pointSize;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;

        return gameObject;
    }

    private void ConnectPoints(Vector2 fromPoint, Vector2 toPoint)
    {
        var gameObject = new GameObject("Connection", typeof(Image));
        gameObject.transform.SetParent(container, false);

        var image = gameObject.GetComponent<Image>();
        image.color = connectionColor;

        Vector2 direction = (toPoint - fromPoint).normalized;
        float distance = Vector2.Distance(fromPoint, toPoint);

        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = fromPoint + direction * distance * 0.5f;
        rectTransform.sizeDelta = new Vector2(distance, connectionWidth);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.localEulerAngles = new Vector3(0f, 0f, GetAngleBetweenPoints(fromPoint, toPoint));
    }

    private float GetAngleBetweenPoints(Vector2 pointA, Vector2 pointB)
    {
        float xDiff = pointB.x - pointA.x;
        float yDiff = pointB.y - pointA.y;

        return Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
    }

    private void PlotGraph(List<float> dataPoints)
    {
        float graphHeight = container.sizeDelta.y;
        float xDelta = 50f;

        for (int itr = 0; itr < dataPoints.Count; itr++)
        {
            float x = itr * xDelta;
            float y = (dataPoints[itr] / maxY) * graphHeight;
            PlotElement(x, y);
        }
    }
}