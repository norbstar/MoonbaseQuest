using System;

using UnityEngine;

// Part 1 https://www.youtube.com/watch?v=tBYl-aSxUe0&ab_channel=Valem
// Part 2 https://www.youtube.com/watch?v=Wk2_MtYSPaM&ab_channel=Valem
// Part 3 https://www.youtube.com/watch?v=8REDoRu7Tsw&ab_channel=Valem
public class RobotKyleRig : MonoBehaviour
{
    [Serializable]
    public class RobotKyleMap
    {
        public Transform target;
        public Transform rigTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;

        public void Map()
        {
            rigTarget.position = target.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = target.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }

    [Header("Mapping")]
    [SerializeField] RobotKyleMap head;
    [SerializeField] RobotKyleMap rightHand;
    
    [Header("Constraints")]
    [SerializeField] Transform headConstraint;
    [SerializeField] float turnSpeed = 1f;

    private Vector3 headBodyOffset;

    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }

    void LateUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;
        // transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.deltaTime * turnSpeed);

        head.Map();
        rightHand.Map();
    }
}