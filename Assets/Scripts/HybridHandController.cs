using UnityEngine;

public class HybridHandController : HandController
{
    [Header("Components")]
    [SerializeField] GameObject body;

    private MeshCollider meshCollider;
    private OnCollisionHandler collisionHandler;

    public override void Awake()
    {
        base.Awake();

        ResolveLocalDependencies();
    }

    private void ResolveLocalDependencies()
    {
        collisionHandler = body.GetComponent<OnCollisionHandler>() as OnCollisionHandler;
        meshCollider = body.GetComponent<MeshCollider>() as MeshCollider;
    }

    void OnEnable()
    {
        collisionHandler.EventReceived += OnCollisionEvent;
    }

    void OnDisable()
    {
        collisionHandler.EventReceived -= OnCollisionEvent;
    }

    public void OnCollisionEvent(OnCollisionHandler.EventType type, GameObject gameObject)
    {
        switch (type)
        {
            case OnCollisionHandler.EventType.OnCollisionEnter:
                break;

            case OnCollisionHandler.EventType.OnCollisionExit:
                break;
        }
    }
    
    protected override void IsGripping(bool gripping)
    {
        meshCollider.enabled = !gripping;
        sphereCollider.enabled = gripping;
    }
}