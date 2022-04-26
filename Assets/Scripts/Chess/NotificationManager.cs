using System.Collections;

using UnityEngine;

using TMPro;

namespace Chess
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI frontTextUI;
        [SerializeField] TextMeshProUGUI rearTextUI;

        public string Text
        {
            get
            {
                return frontTextUI.text;
            }

            set
            {
                frontTextUI.text = value;
                rearTextUI.text = value;
            }
        }

        public void ShowFor(float delaySec)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowForCoroutine(delaySec));
        }

        private IEnumerator ShowForCoroutine(float delaySec)
        {
            yield return new WaitForSeconds(delaySec);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}