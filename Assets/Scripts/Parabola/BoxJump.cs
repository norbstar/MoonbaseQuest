using UnityEngine;

namespace Parabola
{
    public class BoxJump : MonoBehaviour
    {
        [SerializeField] GameObject pointPrefab;
        [SerializeField] int skipFramesPerPoint = 10;
        [SerializeField] float destroyPrefabAfterSec = 5f;
        [SerializeField] float height = 2.5f;
        [SerializeField] float distance = 5f;

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

            animation += Time.deltaTime;
            animation = animation % 5f;

            // transform.position = MathParabola.Parabola(Vector3.zero, Vector3.forward * 10f, 5f, animation / 5f);
            transform.position = MathParabola.Parabola(start, Vector3.forward * distance, height, animation / 5f);

            if (skipFrameCount == skipFramesPerPoint)
            {
                PlotPosition(transform.position);
                skipFrameCount = 0;
            }
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