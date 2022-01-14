using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightColorSequencer : ColorSequencer
{
    private new Light light;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        light = GetComponent<Light>() as Light;
    }

    protected override void SetColor(Color color)
    {
        light.color = color;        
    }
}