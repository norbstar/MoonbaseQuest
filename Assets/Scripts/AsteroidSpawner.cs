using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] Vector3 size;
    [SerializeField] GameObject sponge;

    [Header("The spawn interval")]
    [SerializeField] int spawnInterval = 1;

    [Header("The prefab to spawn")]
    [SerializeField] GameObject prefab;

    [Header("The maximum permissible asteroid speed")]
    [SerializeField] float maxSpeed = 5f;

    [Header("Limit journey length over time")]
    [SerializeField] float journeyLength;

    private float nextSpawn = 0f;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, size);
    }

     // Start is called before the first frame update
    void Start()
    {
        sponge.transform.position -= transform.forward * journeyLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            Spawn();
            nextSpawn = Time.time + spawnInterval;
        }
    }

    private void Spawn()
    {
        var spawnPoint = transform.position + new Vector3(
            Random.Range(-(size.x / 2), (size.x / 2)),
            Random.Range(-(size.y / 2), (size.y / 2)),
            Random.Range(-(size.z / 2), (size.z / 2))
        );

        var rotation = new Quaternion
        {
            x = UnityEngine.Random.Range(0, 360),
            y = UnityEngine.Random.Range(0, 360),
            z = UnityEngine.Random.Range(0, 360)
        };

        var instance = GameObject.Instantiate(prefab, spawnPoint, /*Quaternion.identity*/rotation) as GameObject;
        instance.gameObject.transform.SetParent(transform);

        var rigidBody = instance.GetComponent<Rigidbody>() as Rigidbody;
        rigidBody.velocity = -transform.forward * (Random.Range(maxSpeed / 4, maxSpeed));
    }
}