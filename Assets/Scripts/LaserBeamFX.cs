using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaserBeamFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.05f);
    }
}