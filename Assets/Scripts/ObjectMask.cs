using System.Collections.Generic;

using UnityEngine;

public class ObjectMask : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castRay = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;

        if (Physics.Raycast(castRay, out hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }
    }
}