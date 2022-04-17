using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] LineRenderer line;

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
        line.startWidth = line.endWidth = 0.015f;
        line.positionCount = 2;
    }

    public void ConfigLine(Vector3 endPoint)
    {
        Vector3 startPoint = transform.position;
        line.SetPositions(new Vector3[] { startPoint, endPoint });
    }
}