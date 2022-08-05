using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class ButtonUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Image header;
        public Image Header { get { return header; } }
        [SerializeField] Image icon;
        public Image Icon { get { return icon; } }
        [SerializeField] TextMeshProUGUI textUI;
        public TextMeshProUGUI TextUI { get { return textUI; } }
        [SerializeField] AnnotationUIManager annotation;
        public AnnotationUIManager Annotation { get { return annotation; } }

        [Header("Config")]
        [SerializeField] Color headerHighlightColor;
        public Color HeaderHighlightColor { get { return headerHighlightColor; } }
        [SerializeField] bool overridePointerEnterEvent = false;
        public bool OverridePointerEnterEvent { get  { return overridePointerEnterEvent; } }
        [SerializeField] bool overridePointerExitEvent = false;
        public bool OverridePointerExitEvent { get  { return overridePointerExitEvent; } }

        private Color defaultHeaderColor;
        public Color DefaultHeaderColor { get { return defaultHeaderColor; } }

        private ScaleFXManager scaleFXManager;
        public ScaleFXManager ScaleFXManager { get { return scaleFXManager; } }

        // Start is called before the first frame update
        void Start()
        {
            ResolveDependencies();

            if (header != null)
            {
                defaultHeaderColor = header.color;
            }
       }

        private void ResolveDependencies() => scaleFXManager = GetComponent<ScaleFXManager>() as ScaleFXManager;

        public Color HeaderColor { get { return header.color; } set { header.color = value;  } }
    }
}