using System.Collections.Generic;

using UnityEngine;

public class SurfaceImpactManager : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        spriteRenderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    // Start is called before the first frame update
    void Start()
    {
        var idx = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[idx];
    }
}