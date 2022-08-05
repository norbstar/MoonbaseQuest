using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class ButtonGroupManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<UnityEngine.UI.Button> group;
        public List<UnityEngine.UI.Button> Group { get { return group; } }

        [Header("Config")]
        [SerializeField] int defaultSelection = 0;

        public delegate void ButtonGroupEvent(List<UnityEngine.UI.Button> group);
        public event ButtonGroupEvent EventReceived;

        public UnityEngine.UI.Button Selected
        {
            get
            {
                return selected;
            }

            set
            {
                selected = value;
                selected.interactable = false;
            }
        }

        private UnityEngine.UI.Button selected;

        // Start is called before the first frame update
        void Start()
        {
            foreach (UnityEngine.UI.Button button in group)
            {
                button.onClick.AddListener(delegate {
                    OnValueChanged(button);
                });
            }

            SetByIndex(defaultSelection);
        }

        private void SetByIndex(int index)
        {
            for (int itr = 0; itr < group.Count; itr++)
            {
                var button = group[itr];

                if (itr == index)
                {
                    button.Select();
                    Selected = button;
                }
            }
        }

        public UnityEngine.UI.Button GetByIndex(int index) => group[index];

        public void OnValueChanged(UnityEngine.UI.Button button)
        {
            if (selected != null)
            {
                selected.interactable = true;
            }

            Selected = button;
            EventReceived?.Invoke(group);
        }
    }
}