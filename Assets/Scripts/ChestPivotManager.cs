using UnityEngine;

public class ChestPivotManager : Gizmo
{
    [Header("Pivot")]
    [SerializeField] new Camera camera;
    [SerializeField] bool matchRotation = true;

    private Vector3 relativePosition;

    // Start is called before the first frame update
    void Start()
    {
        relativePosition = camera.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.transform.position - relativePosition;

        if (matchRotation)
        {
            transform.rotation = Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f);
        }
    }
}