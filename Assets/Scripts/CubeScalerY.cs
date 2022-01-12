using UnityEngine;

public class CubeScalerY : MonoBehaviour
{
    [SerializeField] bool automate = false;

    private static Vector3 ScaleChange = new Vector3(0f, -0.01f, 0f);

    // Update is called once per frame
    void Update()
    {
        if (automate)
        {
            transform.localScale += ScaleChange;

            if (transform.localScale.y < 0.5f || transform.localScale.y > 1.0f)
            {
                ScaleChange = -ScaleChange;
            }
        }

        float positionY = -0.5f - (transform.localScale.y * -0.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, positionY, transform.localPosition.z);
    }
}