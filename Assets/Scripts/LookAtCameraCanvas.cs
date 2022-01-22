using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class LookAtCameraCanvas : MonoBehaviour
{
    [SerializeField] new Camera camera;
    
    private Canvas canvas;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        canvas = GetComponent<Canvas>() as Canvas;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePosition = camera.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        canvas.transform.rotation = rotation;
    }
}