using UnityEngine;

public class ProximityBaseController : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] GameObject origin;

    public Camera Camera { get { return camera; } }
    public GameObject Origin { get { return origin; } }

    public float Offset { get { return offset; } set { offset = value; } }
    public bool InRange { get { return inRange; } set { inRange = value; } }

    private float offset = 0.1f;
    private bool inRange;

    private bool isShown;

    // Update is called once per frame
    void Update()
    {
        ShowChildren(inRange);

        if (inRange)
        {
            Vector3 offsetPoint = Vector3.Lerp(origin.transform.position, camera.transform.position, offset);
            Vector3 relativePosition = camera.transform.position - origin.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
            transform.rotation = rotation;
            transform.position = offsetPoint;
        }
    }

    private void ShowChildren(bool show)
    {
        if (isShown == show) return;

        for (int idx = 0; idx < transform.childCount; idx++)
        {
            var child = transform.GetChild(idx);
            child.gameObject.SetActive(show);
        }

        isShown = show;
    }
}