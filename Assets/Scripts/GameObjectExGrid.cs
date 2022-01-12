using System.Collections.Generic;

using UnityEngine;

public class GameObjectExGrid : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform origin;

    // Start is called before the first frame update
    void Start()
    {
        float lineWidth = 0.05f;

        // DrawGrid(lineWidth, 0f);
        // DrawGrid(lineWidth, -4f);

        DrawLine(new Vector3(0f, -8f, 0f), new Vector3(0f, 8f, 0f), lineWidth);
        DrawLine(new Vector3(-8f, 0f, 0f), new Vector3(8f, 0f, 0f), lineWidth);

        var radius = 1f;

        for (int idx = 0; idx <12; idx++)
        {
            DrawSemiCircle(radius, lineWidth);
            DrawSemiCircle(radius, lineWidth, 180f);
            radius += 0.5f;
        }

        DrawCircle(0.25f, 0.25f, Color.white);
        // DrawCircle(0.25f, 0.25f);
        // Test(0.5f, 0.5f);
    }

    private void DrawGrid(float lineWidth, float z = 0f)
    {
        DrawLine(new Vector3(-7f, -8f, z), new Vector3(-7f, 8f, z), lineWidth);
        DrawLine(new Vector3(-6f, -8f, z), new Vector3(-6f, 8f, z), lineWidth);
        DrawLine(new Vector3(-5f, -8f, z), new Vector3(-5f, 8f, z), lineWidth);
        DrawLine(new Vector3(-4f, -8f, z), new Vector3(-4f, 8f, z), lineWidth);
        DrawLine(new Vector3(-3f, -8f, z), new Vector3(-3f, 8f, z), lineWidth);
        DrawLine(new Vector3(-2f, -8f, z), new Vector3(-2f, 8f, z), lineWidth);
        DrawLine(new Vector3(-1f, -8f, z), new Vector3(-1f, 8f, z), lineWidth);
        DrawLine(new Vector3(0f, -8f, z), new Vector3(0f, 8f, z), lineWidth);
        DrawLine(new Vector3(1f, -8f, z), new Vector3(1f, 8f, z), lineWidth);
        DrawLine(new Vector3(2f, -8f, z), new Vector3(2f, 8f, z), lineWidth);
        DrawLine(new Vector3(3f, -8f, z), new Vector3(3f, 8f, z), lineWidth);
        DrawLine(new Vector3(4f, -8f, z), new Vector3(4f, 8f, z), lineWidth);
        DrawLine(new Vector3(5f, -8f, z), new Vector3(5f, 8f, z), lineWidth);
        DrawLine(new Vector3(6f, -8f, z), new Vector3(6f, 8f, z), lineWidth);
        DrawLine(new Vector3(7f, -8f, z), new Vector3(7f, 8f, z), lineWidth);

        DrawLine(new Vector3(-8f, -7f, z), new Vector3(8f, -7f, z), lineWidth);
        DrawLine(new Vector3(-8f, -6f, z), new Vector3(8f, -6f, z), lineWidth);
        DrawLine(new Vector3(-8f, -5f, z), new Vector3(8f, -5f, z), lineWidth);
        DrawLine(new Vector3(-8f, -4f, z), new Vector3(8f, -4f, z), lineWidth);
        DrawLine(new Vector3(-8f, -3f, z), new Vector3(8f, -3f, z), lineWidth);
        DrawLine(new Vector3(-8f, -2f, z), new Vector3(8f, -2f, z), lineWidth);
        DrawLine(new Vector3(-8f, -1f, z), new Vector3(8f, -1f, z), lineWidth);
        DrawLine(new Vector3(-8f, 0f, z), new Vector3(8f, 0f, z), lineWidth);
        DrawLine(new Vector3(-8f, 1f, z), new Vector3(8f, 1f, z), lineWidth);
        DrawLine(new Vector3(-8f, 2f, z), new Vector3(8f, 2f, z), lineWidth);
        DrawLine(new Vector3(-8f, 3f, z), new Vector3(8f, 3f, z), lineWidth);
        DrawLine(new Vector3(-8f, 4f, z), new Vector3(8f, 4f, z), lineWidth);
        DrawLine(new Vector3(-8f, 5f, z), new Vector3(8f, 5f, z), lineWidth);
        DrawLine(new Vector3(-8f, 6f, z), new Vector3(8f, 6f, z), lineWidth);
        DrawLine(new Vector3(-8f, 7f, z), new Vector3(8f, 7f, z), lineWidth);
    }

    private void DrawGrid(Vector3 from, Vector3 to, float lineWidth, float steps)
    {
        Vector2 stepSize = new Vector2
        {
            x = Mathf.Abs(from.x - to.x) / steps,
            y = Mathf.Abs(from.y - to.y) / steps
        };

        for (float step = 0f; step < steps; step += stepSize.x)
        {
            var point = Mathf.Lerp(from.x, to.x, steps / step);
            DrawLine(new Vector3(point, from.y, 0f), new Vector3(point, to.y, 0f), lineWidth);
        }

        for (float step = 0f; step < steps; step += stepSize.y)
        {
            var point = Mathf.Lerp(from.y, to.y, steps / step);
            DrawLine(new Vector3(from.x, point, 0f), new Vector3(to.x, point, 0f), lineWidth);
        }
    }

    private void Test(float radius, float lineWidth)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = origin;
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Line";
        instance.Test(radius, lineWidth);
    }

    private void TestOld()
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = origin;
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Line";
        instance.TestOld(Color.white, Color.white);
    }

    private void DrawLine(Vector3 start, Vector3 end, float lineWidth)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = origin;
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Line";
        instance.DrawGradientLine(start, end, lineWidth);
    }

    private void DrawCircle(float radius, float lineWidth, Color color, float alpha = 1f)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = origin;
        // instance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Circle";
        instance.DrawCircle(radius, lineWidth, color, alpha);
    }

    private void DrawSemiCircle(float radius, float lineWidth, float offset = 0)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = origin;
        // instance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Semi-Circle";
        instance.DrawSemiCircle(radius, lineWidth, offset);
    }
}