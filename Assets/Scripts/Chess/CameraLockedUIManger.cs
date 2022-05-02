using UnityEngine;

namespace Chess
{
    public class CameraLockedUIManger : MonoBehaviour
    {
        [SerializeField] new Camera camera;

        [Header("Synchronization")]
        [SerializeField] bool syncPosition;
        [SerializeField] bool syncRotation;

        // Start is called before the first frame update
        void Start() => Sync();

        // Update is called once per frame
        void Update()
        {
            if (!syncPosition && !syncRotation) return;
            Sync();
        }

        private void Sync()
        {
            if (syncPosition)
            {
                transform.position = camera.transform.position;
            }

            if (syncRotation)
            {
                transform.rotation = camera.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
            }
        }
    }
}