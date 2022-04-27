using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
    [SerializeField] GameObject target;
 
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
        transform.LookAt(target.transform, Vector3.up);
    }
}