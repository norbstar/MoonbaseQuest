using UnityEngine;

public class FlightStickManager : BaseManager
{
    [Header("Config")]
    [SerializeField] GameObject stick;
    [SerializeField] HandAnimationController hand;

    // Start is called before the first frame update
    void Start()
    {
        hand?.SetFloat("Grip", 1f);
    }

    void FixedUpdate()
    {
        Log($"{gameObject.name}.FixedUpdate:Rotation : {transform.rotation.eulerAngles}");
    }
}