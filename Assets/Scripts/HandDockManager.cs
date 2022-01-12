using UnityEngine;

public class HandDockManager : MonoBehaviour
{
    [SerializeField] new Camera camera;

    public bool InUse { get { return inUse; } set { inUse = value; } }

    private bool inUse;
    private Vector3 relativePosition;
    private InConeValidator inConeValidator;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        inConeValidator = GetComponent<InConeValidator>() as InConeValidator;
    }

    // Start is called before the first frame update
    void Start()
    {
        relativePosition = camera.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.transform.position - relativePosition;
    }
}