using UnityEngine;

public class GunReparentTest : MonoBehaviour
{
    [SerializeField] GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = parent.transform;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        transform.localPosition = Vector3.zero;
    }
}