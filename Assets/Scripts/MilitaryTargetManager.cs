using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class MilitaryTargetManager : MonoBehaviour, IInteractableEvent
{
    [Header("Audio")]
    [SerializeField] AudioClip hitClip;

    [Header("Impact")]
    [SerializeField] SurfaceImpactManager surfaceImpactPrefab;

    [Header("Scoring")]
    [SerializeField] ScoreCanvasManager scoreCanvasManager;
    [SerializeField] int points = 10;

    [Header("Debug")]
    [SerializeField] bool enableLogging = false;

    private Rigidbody rigidBody;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
    }

    public void OnActivate(XRGrabInteractable interactable, Vector3 hitPoint)
    {
        Log($"{gameObject.name}.OnActivate:{interactable.name} {hitPoint}");

        AudioSource.PlayClipAtPoint(hitClip, hitPoint, 1.0f);
        var instance = Instantiate(surfaceImpactPrefab.gameObject, hitPoint, Quaternion.identity);
        instance.transform.parent = transform;
        instance.transform.localRotation = transform.localRotation;

        scoreCanvasManager?.AddToScore(points);

        Destroy(instance, 1.0f);
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}