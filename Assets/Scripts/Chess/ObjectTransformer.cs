using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(RootResolver))]
    public class ObjectTransformer : MonoBehaviour, IPropagationEvent
    {
        [SerializeField] GameObject reference;

        [Header("Config")]
        [SerializeField] bool matchRangeToHeight = true;
        [SerializeField] float range = 1f;

        private new Renderer renderer;
        private GameObject rootGameObject;
        private float originY;
        private UpdateEvent updateEvent;

        public class UpdateEvent
        {
            public Vector3 position;
            public float radius;
        }

        void Awake()
        {
            ResolveDependencies();

            if (TryGet.TryGetRootResolver(gameObject, out GameObject rootGameObject))
            {
                this.rootGameObject = rootGameObject;
                originY = rootGameObject.transform.position.y;
            }

            if (matchRangeToHeight)
            {
                range = renderer.bounds.size.y;
            }
        }

        private void ResolveDependencies() => renderer = GetComponent<Renderer>() as Renderer;

        // Update is called once per frame
        void LateUpdate()
        {
            if (updateEvent == null) return;

            float distance = Vector3.Distance(reference.transform.position, updateEvent.position) - updateEvent.radius;
            distance = (distance <= 0) ? Mathf.Abs(distance) : 0f;
            // Debug.Log($"{gameObject.name} Distance : {distance}");

            float value = Mathf.Clamp(distance, 0f, range);
            float normalizedValue = (value - 0f) / (range - 0f);
            // Debug.Log($"{gameObject.name} Value : {value} Normalized Value : {normalizedValue}");

            rootGameObject.transform.position = new Vector3(rootGameObject.transform.position.x, originY + (renderer.bounds.size.y * normalizedValue), rootGameObject.transform.position.z);
        }

        public void OnUpdate(Vector3 position, float radius)
        {
            if (updateEvent == null)
            {
                updateEvent = new UpdateEvent
                {
                    position = position,
                    radius = radius
                };
            }
            else
            {
                updateEvent.position = position;
                updateEvent.radius = radius;
            }
        }
    }
}