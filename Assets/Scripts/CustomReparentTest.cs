using UnityEngine;

public class CustomReparentTest : MonoBehaviour
{
    [SerializeField] GameObject parentA, parentB;

    // Start is called before the first frame update
    void Start()
    {
        for (int idx = 0; idx < parentA.transform.childCount; idx++)
        {
            var child = parentA.transform.GetChild(idx);
            child.parent = parentB.transform;
            child.rotation = Quaternion.Euler(90f, 0f, 0f);
            child.localPosition = Vector3.zero;
        }        
    }
}