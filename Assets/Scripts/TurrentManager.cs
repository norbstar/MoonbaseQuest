using System.Collections;
using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class TurrentManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("References")]
    [SerializeField] GameObject spawnPoint;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip activateClip;
    [SerializeField] AudioClip deactivateClip;
    [SerializeField] AudioClip hitClip;

    [Header("Events")]
    [SerializeField] AnimationCompleteEvent animationCompleteEvent;

    [Header("Config")]
    [SerializeField] float actuationUPS = 50f;
    [SerializeField] float actuationForce = 1000f;

    private new Rigidbody rigidbody;
    private new HingeJoint hingeJoint;
    private float lastActuation;
    private GameObject lastObjectHit;
    private Vector3 lastObjectHitPoint;
    private IFocus lastFocus;
    private GameObject hitPrefabInstance;
    private int mixedLayerMask;
    private bool canFire;
    private Coroutine fireRepeatCoroutine;

    void Awake()
    {
        mixedLayerMask = LayerMask.GetMask("Default");
        canFire = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rigidbody = GetComponent<Rigidbody>() as Rigidbody;
        hingeJoint = GetComponent<HingeJoint>() as HingeJoint;
    }

    void OnEnable()
    {
        animationCompleteEvent.OnComplete += OnFireComplete;
    }

    void OnDisable()
    {
        animationCompleteEvent.OnComplete -= OnFireComplete;
    }

    void FixedUpdate()
    {
        var ray = new Ray(spawnPoint.transform.position, -spawnPoint.transform.right);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, mixedLayerMask))
        {
            var objectHit = hit.transform.gameObject;
            var point = hit.point;

            if (showHits) ShowHit(point);

            if (!GameObject.ReferenceEquals(objectHit, lastObjectHit))
            {
                if (lastFocus != null)
                {
                    lastFocus.LostFocus(gameObject);
                    lastFocus = null;
                }

                if (objectHit.TryGetComponent<IFocus>(out IFocus focus))
                {
                    focus.GainedFocus(gameObject, point);
                    lastFocus = focus;
                }            

                lastObjectHit = objectHit;
            }

            lastObjectHitPoint = point;
        }
        else
        {
            if (lastFocus != null)
            {
                lastFocus.LostFocus(gameObject);
                lastFocus = null;
            }

            hitPrefabInstance?.SetActive(false);
            lastObjectHit = null;
            lastObjectHitPoint = default(Vector3);
        }
    }

    private void ShowHit(Vector3 point)
    {
        if (hitPrefabInstance == null)
        {
            hitPrefabInstance = Instantiate(hitPrefab, point, Quaternion.identity);
        }
        else
        {
            hitPrefabInstance.transform.position = point;
            hitPrefabInstance.SetActive(true);
        }
    }

    public void Activate()
    {
        hingeJoint.useSpring = false;
        rigidbody.useGravity = false;
        AudioSource.PlayClipAtPoint(activateClip, transform.position, 1.0f);
    }

    public void Deactivate()
    {
        AudioSource.PlayClipAtPoint(deactivateClip, transform.position, 1.0f);
        rigidbody.useGravity = true;
        hingeJoint.useSpring = true;
    }

    public void Rotate(float rotationForce, float normalized)
    {
        Log($"{gameObject.name} {className} Rotate:Rotational Force : {rotationForce} Normalized : {normalized}");
        transform.Rotate(-rotationForce, 0f, 0f);
    }

    public void OnActivate()
    {
        Log($"{gameObject.name} {className} OnActivate");
        fireRepeatCoroutine = StartCoroutine(FireRepeat());
    }

    public void OnDeactivate()
    {
        Log($"{gameObject.name} {className} OnDeactivate");

        if (fireRepeatCoroutine != null)
        {
            StopCoroutine(fireRepeatCoroutine);
        }
    }

    private IEnumerator FireRepeat()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Fire()
    {
        if (!canFire) return;

        canFire = false;
        animator.SetTrigger("Fire");
        AudioSource.PlayClipAtPoint(hitClip, transform.position, 1.0f);

        StartCoroutine(PostFireCoroutine());
    }

    private IEnumerator PostFireCoroutine()
    {
        var objectHit = lastObjectHit;
        var objectHitPoint = lastObjectHitPoint;

        CollisionData collisionData = new CollisionData();
        yield return StartCoroutine(ApplyImpactForceCoroutine(collisionData));

        if ((objectHit != null) && (objectHit.TryGetComponent<IInteractableEvent>(out IInteractableEvent interactableEvent)))
        {
            // TODO NOTE the 1st param should be a XRGrabInteractable, but this is incompatible with the turrent fly by wire model
            // ALSO maybe the 2nd argument should be the spawn point
            interactableEvent.OnActivate(null, transform, objectHitPoint, collisionData.Force);
        }
    }

    private IEnumerator ApplyImpactForceCoroutine(CollisionData collisionData)
    {
        var ray = new Ray(spawnPoint.transform.position, spawnPoint.transform.forward);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, mixedLayerMask))
        {
            var objectHit = hit.transform.gameObject;
            var point = hit.point;

            Vector3 direction = point - spawnPoint.transform.position;
            float distance = Vector3.Distance(spawnPoint.transform.position, point);
            float duration = distance / actuationUPS;

            yield return new WaitForSeconds(duration);

            if (objectHit.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                Vector3 force = direction.normalized * actuationForce;
                rigidbody.AddForceAtPosition(force, point);
                
                collisionData.GameObject = objectHit;
                collisionData.Force = force;
                collisionData.Point = point;
            }
        }
    }

    public void OnFireComplete() => canFire = true;
}