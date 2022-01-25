using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaserBeamController : MonoBehaviour
{
    [SerializeField] float force = 100f;
    [SerializeField] float speed = 75f;

    private GameManager gameManager;
    private GameObject parent;
    private Rigidbody rigidBody;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        gameManager = GameManager.GetInstance();
        parent = GameObject.Find("Projectiles");
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = parent.transform;
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // transform.position += transform.forward * Time.deltaTime * speed;
        rigidBody.velocity = transform.forward * Time.deltaTime * force;
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject trigger = collision.gameObject;
        // Debug.Log($"{gameObject.name}.OnCollisionEnter:{trigger.name}");

        HandleInteraction(trigger);
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;
        // Debug.Log($"{gameObject.name}.OnTriggerEnter:{trigger.name}");

        HandleInteraction(trigger);
    }

    private void HandleInteraction(GameObject trigger)
    {
        // Debug.Log($"{gameObject.name}.HandleInteraction:{trigger.name}");

        if (trigger.CompareTag("Asteroid") && gameManager.GameState == GameManager.State.InPlay)
        {
            // Debug.Log($"{gameObject.name}.Trigger Is Valid");
            var damage = trigger.GetComponent<IDamage>() as IDamage;

            if (damage != null)
            {
                damage.ApplyDamage();
            }
            else
            {
                Destroy(trigger);
            }
        }
        
        Destroy(gameObject);
    }
}