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
                value.interactable = false;
                selected = value;
            }
        }

        private UnityEngine.UI.Button selected;

        private AudioClip onClickClip;

        public AudioClip OnClickClip
        {
            get
            {
                return onClickClip;
            }

            set
            {
                onClickClip = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (UnityEngine.UI.Button button in group)
            {
                button.onClick.AddListener(delegate {
                    if (onClickClip != null)
                    {
                        AudioSource.PlayClipAtPoint(onClickClip, Vector3.zero, 1.0f);
                    }

                    OnValueChanged(button);
                });
            }

            Selected = GetByIndex(defaultSelection);
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

        public UnityEngine.UI.Button GetByIndex(int index) => group[index];

        public void OnValueChanged(UnityEngine.UI.Button button)
        {
            if (selected != null)
            {
                selected.interactable = true;
            }

            Selected = button;
            EventReceived(group);
        }
    }
}