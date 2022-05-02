using UnityEngine;

namespace Chess
{
    public class CameraStaticUIManger : MonoBehaviour
    {
        [SerializeField] new Camera camera;

        // Start is called before the first frame update
        void Start()
        {
            transform.position = camera.transform.position;
            transform.rotation = camera.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
        }
    }
}