using UnityEngine;

public class KnifeVisualElement : MonoBehaviour
{
    public void EnableChildRenderers(bool enable)
    {
        foreach (Transform thisTransform in transform)
        {
            var renderer = thisTransform.GetComponent<Renderer>() as Renderer;
            renderer.enabled = enable;
        }
    }
}