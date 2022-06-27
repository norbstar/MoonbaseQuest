using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Chess
{
    public class ButtonUIManager : MonoBehaviour
    {
        [Header("Components")]
         [SerializeField] Image header;
        public Image Header { get { return header; } }
        [SerializeField] Image icon;
        public Image Icon { get { return icon; } }
        [SerializeField] TextMeshProUGUI textUI;
        public TextMeshProUGUI TextUI { get { return textUI; } }
    }
}