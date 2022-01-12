using UnityEngine;

public class CubeScalerZ : MonoBehaviour
{
    [SerializeField] bool automate = false;

    private static Vector3 ScaleChange = new Vector3(0f, 0f, -0.01f);

    // Update is called once per frame
    void Update()
    {
        if (automate)
        {
            transform.localScale += ScaleChange;

            if (transform.localScale.z < 0.5f || transform.localScale.z > 1.0f)
            {
                ScaleChange = -ScaleChange;
            }
        }

        float positionZ = -0.5f - (transform.localScale.z * -0.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, positionZ);
    }
}