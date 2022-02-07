using UnityEngine;

public class LookAtGameObject : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject origin;
    [SerializeField] Vector3 upwards = Vector3.up;
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
        Vector3 relativePosition = target.transform.position - origin.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, upwards);
        transform.rotation = rotation;
    }
}