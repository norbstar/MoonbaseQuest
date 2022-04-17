using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] LineRenderer line;

    [Header("Config")]
    [SerializeField] float lineWidth = 0.01f;

    public bool EnableLine
    {
        get
        {
            return line.gameObject.activeSelf;
        }

        set
        {
            line.gameObject.SetActive(value);
        }
    }

    void Start()
    {
        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;
    }

    public void ConfigLine(Vector3 endPoint)
    {
        Vector3 startPoint = transform.position;
        line.SetPositions(new Vector3[] { startPoint, endPoint });
    }
}