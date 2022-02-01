using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CanvasGraph : MonoBehaviour
{
    [SerializeField] private RectTransform container;

    [Header("Points")]
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Vector2 pointSize = Vector2.one * 5;
    [SerializeField] private Color pointColor = Color.white;
    
    // Time delta
    [SerializeField] private float xDelta = 50f;
       
    // Max value
    [SerializeField] private float yMax = 100f;

    [Header("Connections")]
    [SerializeField] private Color connectionColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float connectionWidth = 1f;

    // Start is called before the first frame update
    void Start()
    {
        List<float> dataPoints = new List<float> { 0f, 22f, 37f, 45f, 43f, 32f, 34f, 43f, 64f, 85f, 100f };
        PlotGraph(dataPoints);
    }

    public GameObject PlotPoint(Vector2 anchoredPosition)
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

    private void PlotGraph(List<float> dataPoints)
    {
        float graphHeight = container.sizeDelta.y;
        
        // Time delta
        float xDelta = 50f;
        
        // Max permissible value
        float yMax = 100f;

        GameObject lastPoint = null;

        for (int itr = 0; itr < dataPoints.Count; itr++)
        {
            float x = itr * xDelta;
            float y = (dataPoints[itr] / yMax) * graphHeight;
            var point = PlotPoint(new Vector2(x, y));

            if (lastPoint != null)
            {
                ConnectPoints(lastPoint.GetComponent<RectTransform>().anchoredPosition, point.GetComponent<RectTransform>().anchoredPosition);
            }

            lastPoint = point;
        }
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
}