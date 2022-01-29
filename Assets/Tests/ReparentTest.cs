using UnityEngine;

public class ReparentTest : MonoBehaviour
{
    [SerializeField] GameObject parentA, parentB;

    // Start is called before the first frame update
    void Start()
    {
        for (int idx = 0; idx < parentA.transform.childCount; idx++)
        {
            var child = parentA.transform.GetChild(idx);
            child.parent = parentB.transform;
        }        
    }
}