using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Components")]
    [SerializeField] Transform tip;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;

    [Header("Config")]
    [SerializeField] float speed = 2000f;

    private MeshRenderer[] renderers;
    private new Rigidbody rigidbody;
    private bool inMotion;
    private Vector3 lastPosition;
    private float alpha;

    public float Alpha
    {
        get
        {
            return alpha;
        }

        set
        {
            SetAlpha(value);
        }
    }

    void Awake()
    {
        renderers = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    private void SetAlpha(Material material, float alpha) => material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
    
    private void SetAlpha(float alpha)
    {
        this.alpha = alpha;

        foreach (MeshRenderer renderer in renderers)
        {
            SetAlpha(renderer.material, alpha);    
        }
    }

    public void Show() => SetAlpha(1f);

    public void Hide() => SetAlpha(0f);

    void FixedUpdate()
    {
        if (!inMotion) return;

        // rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity, transform.up));

        if (Physics.Linecast(lastPosition, tip.position))
        {
            Stop();
        }

        lastPosition = tip.position;
    }

    private void Stop()
    {
        // Log($"{className} {Time.time} Stop");

        inMotion = false;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    public void Fire(float pullValue)
    {
        Log($"{className} {Time.time} Fire PullValue: {pullValue}");

        inMotion = true;
        transform.parent = null;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;

        var force = transform.forward * 2000;//(pullValue * speed);
        // Log($"{className} {Time.time} PullValue: {pullValue} Fire Force: {force}");

        rigidbody.AddForce(force);

        AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);

        Destroy(gameObject, 5f);
    }
}