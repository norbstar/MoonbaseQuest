using UnityEngine;

public class GameObjectExTest : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    private GameObject floor;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        floor = GameObject.Find("Floor");
    }

    // Start is called before the first frame update
    void Start()
    {
        // var circle = new GameObject { name = "Circle" };
        // circle.DrawCircle(1, .02f);

        var radius = 1.5f;

        for (int idx = 0; idx < 3; idx++)
        {
            DrawCircle(radius, 0.05f, Color.white);
            radius += 1.5f;
        }

        DrawLine(Vector3.zero, new Vector3(0f, 8.5f, 0f), 0.025f);
        DrawLine(Vector3.zero, new Vector3(0f, -8.5f, 0f), 0.025f);
        DrawLine(Vector3.zero, new Vector3(8.5f, 0f, 0f), 0.025f);
        DrawLine(Vector3.zero, new Vector3(-8.5f, 0f, 0f), 0.025f);
    }

    private void DrawLine(Vector3 start, Vector3 end, float lineWidth)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = floor.transform;
        // instance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Line";
        instance.DrawLine(start, end, lineWidth);
    }

    private void DrawCircle(float radius, float lineWidth, Color color, float alpha = 1f)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = floor.transform;
        // instance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        instance.transform.Rotate(90f, 0f, 0f);
        instance.name = "Circle";
        instance.DrawCircle(radius, lineWidth, color, alpha);
    }
}