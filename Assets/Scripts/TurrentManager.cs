using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class TurrentManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Config")]
    [SerializeField] AudioClip activateClip;
    [SerializeField] AudioClip deactivateClip;
    [SerializeField] AudioClip fireClip;

    private new Rigidbody rigidbody;
    private new HingeJoint hingeJoint;

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

    public void Fire()
    {
        Log($"{gameObject.name} {className} Fire");
        AudioSource.PlayClipAtPoint(fireClip, transform.position, 1.0f);
    }
}