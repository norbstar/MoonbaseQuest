using UnityEngine;

namespace Chess
{
    public class ZoneConstraintManager : MonoBehaviour
    {
        [SerializeField] new Camera camera;

        private bool inZone;
        private Vector3 cachedPosition;

        // Start is called before the first frame update
        void Start() => cachedPosition = camera.transform.localPosition;

        // Update is called once per frame
        void Update()
        {
            if (!inZone)
            {
                camera.transform.localPosition = cachedPosition;
            }
            else
            {
                cachedPosition = camera.transform.localPosition;
            }
        }

        public void OnTriggerEnter(Collider collider) => inZone = true;

        public void OnTriggerExit(Collider collider) => inZone = false;
    }
}