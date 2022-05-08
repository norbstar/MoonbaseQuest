using UnityEngine;

public class AndroidController : MultiFocusableManager
{
    // [SerializeField] GameObject target;
    // [SerializeField] float force = 100f;

    // [Header("Eyes")]
    // [SerializeField] GameObject leftEyeSocket;
    // [SerializeField] GameObject rightEyeSocket;

    // [SerializeField] ObservableUI observableUI;

    private Rigidbody rigidBody;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 direction;
        // float distance;

        // direction = (target.transform.position - leftEyeSocket.transform.position).normalized;
        // distance = Vector3.Distance(leftEyeSocket.transform.position, target.transform.position);
        // leftEyeSocket.transform.rotation = Quaternion.LookRotation(direction);
        // // Debug.Log($"Left Eye Direction : {direction} Distance : {distance}");
        // Debug.DrawLine(leftEyeSocket.transform.position + direction, leftEyeSocket.transform.position, Color.red);

        // direction = (target.transform.position - rightEyeSocket.transform.position).normalized;
        // distance = Vector3.Distance(rightEyeSocket.transform.position, target.transform.position);
        // rightEyeSocket.transform.rotation = Quaternion.LookRotation(direction);
        // // Debug.Log($"Right Eye Direction : {direction} Distance : {distance}");
        // Debug.DrawLine(rightEyeSocket.transform.position + direction, rightEyeSocket.transform.position, Color.blue);
    }

    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     rigidBody.velocity = transform.forward * Time.deltaTime * force;
    // }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject trigger = collision.gameObject;
        // Debug.Log($"{gameObject.name}.OnCollisionEnter:{trigger.name}");
    }
}