using UnityEngine;

public class CubeScalerTest : MonoBehaviour
{
    private GameObject cube;
    private Vector3 scaleChange, positionChange;
    private Renderer rend;

    void Awake()
    {    
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // Create a cube at the origin.
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0);

        // Change the cube color.
        rend = cube.GetComponent<Renderer>();
        rend.material = Resources.Load<Material>("Materials/LaserBeam_Green");

        // Create a plane and move down by 0.5.
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(0, -0.5f, 0);
        plane.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        // Change the floor color.
        rend = plane.GetComponent<Renderer>();
        rend.material = Resources.Load<Material>("Materials/LaserBeam_Blue");

        scaleChange = new Vector3(0f, -0.01f, 0f);
        positionChange = new Vector3(0.0f, -0.005f, 0.0f);
    }

    void Update()
    {
        cube.transform.localScale += scaleChange;
        cube.transform.position += positionChange;

        // Move upwards when the sphere hits the floor or downwards
        // when the sphere scale extends 1.0f.
        if (cube.transform.localScale.y < 0.5f || cube.transform.localScale.y > 1.0f)
        {
            scaleChange = -scaleChange;
            positionChange = -positionChange;
        }
    }
}