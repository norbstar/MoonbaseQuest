using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class ToggleGroupManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<Toggle> group;
        public List<Toggle> Group { get { return group; } }

        public delegate void ToggleGroupEvent(List<Toggle> group);
        public event ToggleGroupEvent EventReceived;

        private AudioClip onToggleClip;

        public AudioClip OnToggleClip
        {
            get
            {
                return onToggleClip;
            }

            set
            {
                onToggleClip = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (Toggle toggle in group)
            {
                toggle.onValueChanged.AddListener(delegate {
                    if (onToggleClip != null)
                    {
                        AudioSource.PlayClipAtPoint(onToggleClip, Vector3.zero, 1.0f);
                    }

                    OnValueChanged(toggle);
                });
            }
        }

        public void SetByIndex(int index)
        {
            for (int itr = 0; itr < group.Count; itr++)
            {
                var toggle = group[itr];
                toggle.isOn = (itr == index);
            }
        }

        public void OnValueChanged(Toggle toggle) => EventReceived(group);
    }
}