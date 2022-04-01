using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class MilitaryTargetManager : BaseManager, IInteractableEvent
{
    [Header("Audio")]
    [SerializeField] AudioClip hitClip;

    [Header("Impact")]
    [SerializeField] bool autoDestroy = true;
    [SerializeField] SurfaceImpactManager surfaceImpactPrefab;
    [SerializeField] float destroyAfter = 1f;
    [SerializeField] int impactLimit = 10;

    [Header("Scoring")]
    [SerializeField] ScoreCanvasManager scoreCanvasManager;
    [SerializeField] int points = 10;

    private static List<GameObject> impacts;

    private Rigidbody rigidBody;
    private MilitaryTargetPointMap pointMap;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rigidBody = GetComponent<Rigidbody>() as Rigidbody;
        pointMap = GetComponent<MilitaryTargetPointMap>() as MilitaryTargetPointMap;
    }

    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force)
    {
        Log($"{gameObject.name}.OnActivate:{interactable.name} Hit Point : {hitPoint} Force : {force}");

        AudioSource.PlayClipAtPoint(hitClip, hitPoint, 1.0f);
        var instance = Instantiate(surfaceImpactPrefab.gameObject, hitPoint, Quaternion.identity);
        instance.transform.parent = transform;
        instance.transform.localRotation = transform.localRotation;

        if ((pointMap != null) && (pointMap.TryGetValueFromPoint(instance.transform, out var value)))
        {
            scoreCanvasManager?.AddToScore(value);
        }
        else
        {
            scoreCanvasManager?.AddToScore(points);
        }

        if (autoDestroy)
        {
            Destroy(instance, destroyAfter);
        }
        else
        {
            AddToImpacts(instance);
        }
    }

    private void AddToImpacts(GameObject gameObject)
    {
        if (impacts == null)
        {
            impacts = new List<GameObject>();
        }

        impacts.Add(gameObject);

        if (impacts.Count > impactLimit)
        {
            gameObject = impacts[0];
            impacts.RemoveAt(0);
            Destroy(gameObject);
        }
    }
}