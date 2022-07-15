using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public abstract class GroupManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<UnityEngine.UI.Button> group;
        public List<UnityEngine.UI.Button> Group { get { return group; } }

        public delegate void ButtonGroupEvent(List<UnityEngine.UI.Button> group);
        public event ButtonGroupEvent EventReceived;

        public UnityEngine.UI.Button Selected { get { return selected; } }

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
        }

        public void SetByIndex(int index)
        {
            for (int itr = 0; itr < group.Count; itr++)
            {
                var button = group[itr];
                if (itr == index)
                {
                    button.Select();
                }
            }
        }

        public void OnValueChanged(UnityEngine.UI.Button button)
        {
            if (selected != null)
            {
                selected.interactable = true;
            }

            button.interactable = false;
            selected = button;

            EventReceived(group);
        }
    }
}