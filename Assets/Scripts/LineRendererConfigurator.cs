using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRInteractorLineVisual))]
public class LineRendererConfigurator : MonoBehaviour
{
    [SerializeField] float lineWidth = 0.0025f;

    private XRInteractorLineVisual lineVisual;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        lineVisual = GetComponent<XRInteractorLineVisual>() as XRInteractorLineVisual;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        lineVisual.lineWidth = lineWidth;
    }
}