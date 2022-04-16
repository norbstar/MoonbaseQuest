using UnityEngine;

namespace Parabola
{
    public class BoxJump : MonoBehaviour
    {
        [SerializeField] float height = 2.5f;
        [SerializeField] float distance = 5f;

        [Header("Config")]
        [SerializeField] bool enableTracking;
        [SerializeField] GameObject pointPrefab;
        [SerializeField] int skipFramesPerPoint = 10;
        [SerializeField] float destroyPrefabAfterSec = 5f;

        protected new float animation;
        private int skipFrameCount;
        private Vector3 start;

        // Start is called before the first frame update
        void Start()
        {
            start = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            ++skipFrameCount;

            animation += Time.deltaTime * (1f / distance);
            animation = animation % 0.5f;

            // transform.position = MathParabola.Parabola(Vector3.zero, Vector3.forward * 10f, 2.5f, animation / 1f);
            transform.position = MathParabola.Parabola(start, start + (Vector3.forward * distance), height, animation / 0.5f);
            // transform.position = Move(start, Vector3.forward * distance, animation / 1f);
            // transform.position = Move(start, transform.position + (Vector3.forward * distance), animation / 1f);
            // transform.position = Move(start, start + (Vector3.forward * distance), animation / 1f);

            if (enableTracking)
            {
                if (skipFrameCount == skipFramesPerPoint)
                {
                    PlotPosition(transform.position);
                    skipFrameCount = 0;
                }
            }
        }

        private Vector3 Move(Vector3 start, Vector3 end, float timeframe)
        {
            return Vector3.Lerp(start, end, timeframe);
        }

        private void PlotPosition(Vector3 point)
        {
            GameObject instance = Instantiate(pointPrefab, point, Quaternion.identity);
            instance.transform.localScale = Vector3.one * 0.1f;
            // instance.transform.parent = transform;

            Destroy(instance, destroyPrefabAfterSec);
        }
    }
}