using System.Collections.Generic;

using UnityEngine;

public class SimpleObjectMask : MonoBehaviour
{
    [SerializeField] List<GameObject> objs;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in objs)
        {
            obj.GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }
    }
}