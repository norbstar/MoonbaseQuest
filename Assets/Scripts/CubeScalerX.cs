using UnityEngine;

public class CubeScalerX : MonoBehaviour
{
    [SerializeField] bool automate = false;

    private static Vector3 ScaleChange = new Vector3(-0.01f, 0f, 0f);

    // Update is called once per frame
    void Update()
    {
        if (automate)
        {
            transform.localScale += ScaleChange;

            if (transform.localScale.x < 0.5f || transform.localScale.x > 1.0f)
            {
                ScaleChange = -ScaleChange;
            }
        }

        float positionX = -0.5f - (transform.localScale.x * -0.5f);
        transform.localPosition = new Vector3(positionX, transform.localPosition.y, transform.localPosition.z);
    }
}