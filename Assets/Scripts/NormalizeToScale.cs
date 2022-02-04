using UnityEngine;

public class NormalizeToScale : MonoBehaviour
{
    public virtual void Awake()
    {
        var referenceScale = transform.parent.lossyScale;

        var normalizedScale = new Vector3
        {
            x = 1f / (referenceScale.x / 0.1f),
            y = 1f / (referenceScale.y / 0.1f),
            z = 1f / (referenceScale.z / 0.1f)
        };

        transform.localScale = normalizedScale;
    }
}