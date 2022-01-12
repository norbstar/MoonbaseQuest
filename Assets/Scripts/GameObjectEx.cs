using UnityEngine;

public static class GameObjectEx
{
    public static void DrawLine(this GameObject container, Vector3 start, Vector3 end, float lineWidth)
    {
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        line.useWorldSpace = false;
        // line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = Color.white;
        line.endColor = new Color(0f, 0f, 0f, 0f);
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    public static void DrawGradientLine(this GameObject container, Vector3 start, Vector3 end, float lineWidth)
    {
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        // line.useWorldSpace = false;
        // line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        Vector3[] positions = new Vector3[3];
        positions[0] = start;
        positions[1] = Vector3.Lerp(start, end, 0.5f);
        positions[2] = end;
        line.positionCount = positions.Length;
        line.SetPositions(positions);

        Gradient gradient = new Gradient();

        // GradientColorKey[] colorKey = new GradientColorKey[4];
        // colorKey[0].color = new Color(1f, 1f, 1f);
        // colorKey[0].time = 0.0f;
        // colorKey[1].color = new Color(1f, 1f, 1f);
        // colorKey[1].time = 0.1f;
        // colorKey[2].color = new Color(1f, 1f, 1f);
        // colorKey[2].time = 0.9f;
        // colorKey[3].color = new Color(1f, 1f, 1f);
        // colorKey[3].time = 1.0f;

        // colorKey[0].color = new Color(1f, 0f, 0f, 0f);
        // colorKey[0].time = 0.0f;
        // colorKey[1].color = Color.white;
        // colorKey[1].time = 0.1f;
        // colorKey[2].color = Color.white;
        // colorKey[2].time = 0.9f;
        // colorKey[3].color = new Color(0f, 0f, 1f, 0f);
        // colorKey[3].time = 1.0f;

        // GradientAlphaKey[] alphaKey = new GradientAlphaKey[4];
        // alphaKey[0].alpha = 0.0f;
        // alphaKey[0].time = 0.0f;
        // alphaKey[1].alpha = 255f;
        // alphaKey[1].time = 0.1f;
        // alphaKey[2].alpha = 255f;
        // alphaKey[2].time = 0.9f;
        // alphaKey[3].alpha = 0.0f;
        // alphaKey[3].time = 1.0f;

        // GradientAlphaKey[] alphaKey = new GradientAlphaKey[4];
        // alphaKey[0].alpha = 0.0f;
        // alphaKey[0].time = 0.0f;
        // alphaKey[1].alpha = 1.0f;
        // alphaKey[1].time = 0.1f;
        // alphaKey[2].alpha = 1.0f;
        // alphaKey[2].time = 0.9f;
        // alphaKey[3].alpha = 0.0f;
        // alphaKey[3].time = 1.0f;

        // GradientAlphaKey[] alphaKey = new GradientAlphaKey[0];
        // gradient.SetKeys(colorKey, alphaKey);

        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        line.colorGradient = gradient;
        // line.positionCount = 2;
        // line.SetPosition(0, start);
        // line.SetPosition(1, end);
    }

    public static void DrawSemiCircle(this GameObject container, float radius, float lineWidth, float offset = 0)
    {
        var segments = 180;
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 0.25f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(Color.white, 0.75f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(1.0f, 0.25f),
                new GradientAlphaKey(0.0f, 0.5f),
                new GradientAlphaKey(1.0f, 0.75f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        line.colorGradient = gradient;
        line.positionCount = segments + 1;

        // Add an extra point to make the startpoint and endpoint the same to close the circle
        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for (int idx = 0; idx < pointCount; idx++)
        {
            var rad = Mathf.Deg2Rad * (offset + idx * 180f / segments);
            points[idx] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0f);
        }

        line.SetPositions(points);
    }

    public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color color, float alpha = 1f)
    {
        var segments = 360;
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        Gradient gradient = new Gradient();

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = color;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = alpha;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = alpha;
        alphaKey[1].time = 0.0f;

        // GradientAlphaKey[] alphaKey = new GradientAlphaKey[6];
        // alphaKey[0].alpha = 1.0f;
        // alphaKey[0].time = 0.0f;
        // alphaKey[1].alpha = 1.0f;
        // alphaKey[1].time = 0.25f;
        // alphaKey[2].alpha = 0.0f;
        // alphaKey[2].time = 0.251f;
        // alphaKey[3].alpha = 0.0f;
        // alphaKey[3].time = 0.75f;
        // alphaKey[4].alpha = 1.0f;
        // alphaKey[4].time = 0.751f;
        // alphaKey[5].alpha = 1.0f;
        // alphaKey[5].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        // gradient.SetKeys(
        //     new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(Color.white, 1.0f) },
        //     new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
        // );

        // gradient.SetKeys(
        //     new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 0.25f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(Color.white, 0.75f), new GradientColorKey(Color.white, 1.0f) },
        //     new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.25f), new GradientAlphaKey(0.0f, 0.5f), new GradientAlphaKey(1.0f, 0.75f), new GradientAlphaKey(0.0f, 1.0f) }
        // );

        // gradient.SetKeys(
        //     new GradientColorKey[]
        //     {
        //         new GradientColorKey(Color.white, 0.0f),
        //         new GradientColorKey(Color.white, 0.125f),
        //         new GradientColorKey(Color.white, 0.25f),
        //         new GradientColorKey(Color.white, 0.375f),
        //         new GradientColorKey(Color.white, 0.5f),
        //         new GradientColorKey(Color.white, 0.625f),
        //         new GradientColorKey(Color.white, 0.75f),
        //         new GradientColorKey(Color.white, 0.875f),
        //         new GradientColorKey(Color.white, 1.0f)
        //     },
        //     new GradientAlphaKey[]
        //     {
        //         new GradientAlphaKey(0.0f, 0.0f),
        //         new GradientAlphaKey(1.0f, 0.125f),
        //         new GradientAlphaKey(0.0f, 0.25f),
        //         new GradientAlphaKey(1.0f, 0.375f),
        //         new GradientAlphaKey(0.0f, 0.5f),
        //         new GradientAlphaKey(1.0f, 0.625f),
        //         new GradientAlphaKey(0.0f, 0.75f),
        //         new GradientAlphaKey(1.0f, 0.875f),
        //         new GradientAlphaKey(0.0f, 1.0f)
        //     }
        // );

        line.colorGradient = gradient;

        // Add an extra point to make the startpoint and endpoint the same to close the circle
        int pointCount = segments + 2;
        line.positionCount = pointCount;
        var points = new Vector3[pointCount];

        for (int idx = 0; idx < pointCount; idx++)
        {
            var rad = Mathf.Deg2Rad * (idx * 360f / segments);
            // Debug.Log($"Rad : {rad}");
            float x = Mathf.Sin(rad) * radius;
            float y = Mathf.Cos(rad) * radius;
            // Debug.Log($"[{idx}] X : {x} Y : {y}");
            points[idx] = new Vector3(x, y, 0f);
        }

        line.SetPositions(points);
    }

    public static void Test(this GameObject container, float radius, float lineWidth)
    {
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.white;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[0];
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKey, alphaKey);

        int geometicCount = 360;
        int pointCount = geometicCount + 2;

        line.positionCount = pointCount;
        float rad, x, y;

        for (int idx = 0; idx < pointCount; idx++)
        {
            rad = Mathf.Deg2Rad * (idx * 360f / geometicCount);
            x = Mathf.Sin(rad) * radius;
            y = Mathf.Cos(rad) * radius;
            // Debug.Log($"X : {x} Y : {y}");

            line.SetPosition(idx, new Vector3(x, y, 0f));
        }
    }

    public static void TestOld(this GameObject container, Color fromColor, Color toColor)
    {
        var line = container.GetComponent<LineRenderer>() as LineRenderer;

        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
        }

        // line.material = new Material(Shader.Find("Sprites/Default"));

        // Set some positions
        Vector3[] positions = new Vector3[3];
        positions[0] = new Vector3(-2.0f, -2.0f, 0.0f);
        positions[1] = new Vector3(0.0f,  2.0f, 0.0f);
        positions[2] = new Vector3(2.0f, -2.0f, 0.0f);
        line.positionCount = positions.Length;
        line.SetPositions(positions);

        Gradient gradient = new Gradient();
        
        // A simple 2 color gradient with a fixed alpha of 1.0f.
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(fromColor, 0.0f), new GradientColorKey(Color.blue, 0.5f), new GradientColorKey(toColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        line.colorGradient = gradient;
    }
}