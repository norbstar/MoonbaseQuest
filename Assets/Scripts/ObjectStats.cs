using UnityEngine;

public class ObjectStats : MonoBehaviour
{
    [SerializeField] private Vector3 worldPosition, localPosition;

    // Start is called before the first frame update
    void Start()
    {
        worldPosition = transform.position;
        localPosition = transform.localPosition;
    }
}