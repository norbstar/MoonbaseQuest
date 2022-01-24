using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] GameObject origin;
    [SerializeField] bool enableRefresh = false;
    
    protected virtual void Awake()
    {
        FaceToCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enableRefresh) return;
        FaceToCamera();
    }

    private void FaceToCamera()
    {
        Vector3 relativePosition = camera.transform.position - origin.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = rotation;
    }
}