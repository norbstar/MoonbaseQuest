using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    [RequireComponent(typeof(ScaleFXManager))]
    public class ButtonUIManager : MonoBehaviour
    {
        [Header("Components")]
         [SerializeField] Image header;
        public Image Header { get { return header; } }
        [SerializeField] Image icon;
        public Image Icon { get { return icon; } }
        [SerializeField] TextMeshProUGUI textUI;
        public TextMeshProUGUI TextUI { get { return textUI; } }

        [Header("Config")]
        [SerializeField] Color headerHighlightColor;
        public Color HeaderHighlightColor { get { return headerHighlightColor; } }

        private Color defaultHeaderColor;
        public Color DefaultHeaderColor { get { return defaultHeaderColor; } }

        private ScaleFXManager scaleFXManager;
        public ScaleFXManager ScaleFXManager { get { return scaleFXManager; } }

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