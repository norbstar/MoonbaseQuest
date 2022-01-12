using UnityEngine;
using UnityEngine.UI;

public class ColorFade : MonoBehaviour
{
    Color[] colors = new Color[]{
      new Color(0, 1, 0),
    //   new Color(0, 1, 1),
    //   new Color(1, 0.6f, 0),
      new Color(1, 0, 0)};

    private void Awake()
    {
        var hueTex = new Texture2D(colors.Length, 1);
        hueTex.SetPixels(colors);
        hueTex.Apply();

        var slider = transform.GetComponent<Slider>();
        slider.fillRect.GetComponent<Image>().sprite = Sprite.Create(hueTex, new Rect(Vector2.zero, new Vector2(colors.Length, 1)), Vector2.one * 0.5f);
    }

    // Start is called before the first frame update
    void Start() { }
}