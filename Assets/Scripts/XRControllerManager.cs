using UnityEngine;

public class XRControllerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
        for (int itr = 0; itr < gameObject.transform.childCount; itr++)
        {
            var child = gameObject.transform.GetChild(itr);
            child.gameObject.SetActive(false);
        }
        #endif
    }
}