using UnityEngine;

public class RotateOnZAxis : MonoBehaviour
{
    [SerializeField] GameObject target;

    // private GameObject child;

    // void Awake()
    // {
    //     child = transform.GetChild(0).gameObject;
    //     // Debug.Log($"Child: {child.name}");
    // }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(target.transform.position);
        transform.LookAt(transform.position - (target.transform.position - transform.position));

        // Quaternion localRotation = child.transform.localRotation;
        // localRotation.z = target.transform.localRotation.z;
        // child.transform.localRotation = localRotation;
    }
}
