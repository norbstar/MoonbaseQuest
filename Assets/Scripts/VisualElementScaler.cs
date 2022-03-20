using UnityEngine;

public class VisualElementScaler : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        if (prefab != null)
        {
            Transform oldParent = transform.parent;
            transform.parent = null;
            transform.localScale = prefab.transform.localScale;
            transform.parent = oldParent;
        }
    }
}