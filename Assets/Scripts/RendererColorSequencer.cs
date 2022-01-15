using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RendererColorSequencer : ColorSequencer
{
    private new Renderer renderer;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<Renderer>() as Renderer;
    }

    protected override void SetColor(Color color)
    {
        renderer.material.color = color;        
    }
}