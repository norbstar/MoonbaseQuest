using UnityEngine;

using TMPro;

namespace SimonSays
{
    public class SimonSaysUIManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainTextUI;
        public string MainTextUI
        {
            get
            {
                return mainTextUI.text;
            }

            set
            {
                mainTextUI.text = value;
            }
        }

        [SerializeField] TextMeshProUGUI scoreTextUI;
        public string ScoreTextUI
        {
            get
            {
                return scoreTextUI.text;
            }

            set
            {
                scoreTextUI.text = value;
            }
        }
    }
}