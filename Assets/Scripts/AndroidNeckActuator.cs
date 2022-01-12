using UnityEngine;

public class AndroidNeckActuator : MonoBehaviour
{
    // [SerializeField] float smooth = 5f;

    // The target marker.
    public Transform target;

    // Angular speed in radians per sec.
    public float speed = 1.0f;

    // private Quaternion target;

    // Start is called before the first frame update
    void Start()
    {
        // transform.position = new Vector3(transform.position.x, 45f, transform.position.z);       
        // transform.rotation = Quaternion.Euler(0f, 5f, 0f);
        // target = Quaternion.Euler(transform.rotation.x, 45f, transform.rotation.z);

        // Adjust axis left 45 degrees
        // transform.rotation = transform.rotation * Quaternion.AngleAxis(45, Vector3.up);

        // Adjust axis right 45 degrees
        // transform.rotation = transform.rotation * Quaternion.AngleAxis(45, -Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        // transform.position = new Vector3(transform.position.x, transform.position.y + (0.1f * Time.deltaTime) , transform.position.z);       

        // transform.Rotate(0f, 1f * singleStep, 0f);

#if false
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
#endif
    }
}