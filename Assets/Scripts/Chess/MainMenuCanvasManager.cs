using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

namespace Chess
{
    public class MainMenuCanvasManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] GameObject newButton;
        [SerializeField] GameObject loadButton;
        [SerializeField] GameObject exitButton;
        // [SerializeField] GameObject panel;
        
        [Header("Audio")]
        [SerializeField] AudioClip onHoverClip;

        [Header("Configure")]
        [SerializeField] Color onHoverColor;

        // private Image panelImage;
        // private Color defaultBgColor;
        private Color defaultTextColor;

        // void Awake()
        // {
        //     ResolveDependencies();

        //     defaultBgColor = panelImage.color;
        // }

        // private void ResolveDependencies()
        // {
        //     panelImage = panel.GetComponent<Image>() as Image;
        // }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var gameObject = ((PointerEventData) eventData).pointerEnter;
           
            if (gameObject.transform.parent.name.Equals("New"))
            {
                newButton.transform.localScale = new Vector3(1.1f, 1.1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                defaultTextColor = text.color;
                text.color = onHoverColor;
            }
            else if (gameObject.transform.parent.name.Equals("Load"))
            {
                loadButton.transform.localScale = new Vector3(1.1f, 1.1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                defaultTextColor = text.color;
                text.color = onHoverColor;
            }
            else if (gameObject.transform.parent.name.Equals("Exit"))
            {
                exitButton.transform.localScale = new Vector3(1.1f, 1.1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                defaultTextColor = text.color;
                text.color = onHoverColor;
            }

            AudioSource.PlayClipAtPoint(onHoverClip, transform.position, 1.0f);
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var gameObject = ((PointerEventData) eventData).pointerEnter;

            if (gameObject.transform.parent.name.Equals("New"))
            {
                newButton.transform.localScale = new Vector3(1f, 1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                text.color = defaultTextColor;
            }
            else if (gameObject.transform.parent.name.Equals("Load"))
            {
                loadButton.transform.localScale = new Vector3(1f, 1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                text.color = defaultTextColor;
            }
            else if (gameObject.transform.parent.name.Equals("Exit"))
            {
                exitButton.transform.localScale = new Vector3(1f, 1f, 0.25f);
                
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>() as TextMeshProUGUI;
                text.color = defaultTextColor;
            }
        }
        
        public void OnClickButton()
        {
            // TODO
        }
    }
}