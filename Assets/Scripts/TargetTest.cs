using UnityEngine;

public class TargetTest : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject source;
    [SerializeField] GameObject target;
    [SerializeField] bool useLateUpdate;

    // Update is called once per frame
    void Update()
    {
        if (useLateUpdate) return;

        source.transform.LookAt(target.transform.position);
    }
    
    void LateUpdate()
    {
        if (!useLateUpdate) return;
        // var lookPos = source.transform.position - target.transform.position;
        // var rotation = Quaternion.LookRotation(lookPos);
        // source.transform.rotation = rotation;

        source.transform.LookAt(target.transform.position);
        // source.transform.LookAt(source.transform.position - (target.transform.position - source.transform.position));
    }
}