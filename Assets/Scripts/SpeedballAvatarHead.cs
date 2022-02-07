using UnityEngine;

public class SpeedballAvatarHead : MonoBehaviour
{
    [SerializeField] private Transform rootObject, followObject;
    [SerializeField] private Vector3 positionOffset, rotationOffset;

    private Vector3 headBodyOffset;

    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = rootObject.position - followObject.position;
    }

    void LateUpdate()
    {
        rootObject.position = transform.position + headBodyOffset;
        rootObject.forward = Vector3.ProjectOnPlane(followObject.up, Vector3.up).normalized;

        transform.position = followObject.TransformPoint(positionOffset);
        transform.rotation = followObject.rotation * Quaternion.Euler(rotationOffset);
    }
}