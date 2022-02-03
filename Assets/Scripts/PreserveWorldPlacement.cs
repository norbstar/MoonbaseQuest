using UnityEngine;

public class PreserveWorldPlacement : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        Debug.Log($"Original Position: {originalPosition} Original Rotation : {originalRotation}");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        Debug.Log($"Position: {transform.position} Rotation : {transform.rotation}");
    }
}