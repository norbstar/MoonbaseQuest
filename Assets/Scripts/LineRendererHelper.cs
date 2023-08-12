using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererHelper : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float lineWidth = 0.01f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;
    }

    public bool Active {
        get
        {
            return line.gameObject.activeSelf;
        }

        set
        {
            line.gameObject.SetActive(value);
        }
    }

    public void ConfigLine(Vector3 start, Vector3 end) => line.SetPositions(new Vector3[] { start, end });
}