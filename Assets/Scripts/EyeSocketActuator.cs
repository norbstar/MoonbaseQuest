using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EyeSocketActuator : MonoBehaviour
{
    [SerializeField] GameObject eye;

    public GameObject Eye { get { return eye; } }

    private new SpriteRenderer renderer;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        renderer = eye.GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    public void SetColor(Color color)
    {
        renderer.color = color;
    }
}