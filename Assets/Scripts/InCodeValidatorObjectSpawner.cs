using UnityEngine;

public class InCodeValidatorObjectSpawner : MonoBehaviour
{
    [SerializeField] int spawnCount;

    [SerializeField] float radius;

    // Start is called before the first frame update
    void Start()
    {
        for (int idx = 0; idx < spawnCount; idx++)
        {
            var instance = new GameObject();
            instance.transform.position = Random.insideUnitSphere * radius;
            instance.AddComponent(typeof(InConeValidatorObject));
        }
    }
}
