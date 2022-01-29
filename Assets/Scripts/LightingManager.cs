using UnityEngine;

// Demonstrates the use of the context menu annotation which is used here
// to invoke a function from within the editor without the need to launch
// the scene.
public class LightingManager : MonoBehaviour
{
    [ContextMenu("Enable Lighting")]
    public void EnableLighting() => Enable(true);

    [ContextMenu("Disable Lighting")]
    public void DisableLighting() => Enable(false);

    private void Enable(bool enable)
    {
        foreach (Transform transform in transform)
        {
            if (transform.TryGetComponent<Light>(out Light light))
            {
                light.enabled = enable;
            }
        }
    }
}