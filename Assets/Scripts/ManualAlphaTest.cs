using UnityEngine;

public class ManualAlphaTest : MonoBehaviour
{
    [Range (0f, 1f)]
    [SerializeField] float fadeValue = 1f;
    [SerializeField] Color fadeColor = new Color(0, 0, 0, 0);

    private Material material;
    private Color color;

    void Start ()
     {
         material = GetComponent<Renderer>().material;
         color = material.color;
     }

     void Update()
     {
         material.color = fadeColor * fadeValue;
     }
}