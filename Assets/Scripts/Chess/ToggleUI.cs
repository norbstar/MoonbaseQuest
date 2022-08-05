using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class ToggleUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] AnnotationUIManager annotation;
        public AnnotationUIManager Annotation { get { return annotation; } }

        private ScaleFXManager scaleFXManager;
        public ScaleFXManager ScaleFXManager { get { return scaleFXManager; } }

        // Start is called before the first frame update
        void Start() => ResolveDependencies();

        private void ResolveDependencies() => scaleFXManager = GetComponent<ScaleFXManager>() as ScaleFXManager;
    }
}