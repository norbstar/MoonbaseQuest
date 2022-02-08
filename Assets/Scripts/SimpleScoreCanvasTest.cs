using UnityEngine;

public class SimpleScoreCanvasTest : GizmoManager
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject target;

    // Update is called once per frame
    void Update()
    {
        FaceToCamera(target);
    }

    private void FaceToCamera(GameObject gameObject)
    {
        Vector3 relativePosition = transform.position - gameObject.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = rotation;
    }
}