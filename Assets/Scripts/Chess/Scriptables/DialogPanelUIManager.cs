// using System.Collections;

using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.EventSystems;

using TMPro;

namespace Chess
{
    public class DialogPanelUIManager : MonoBehaviour/*, IPointerEventListener/*,TextReceiver*/
    {
        [Header("Components")]
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI textUI;

        [Header("Config")]
        [SerializeField] float onsetDelay = 0.5f;

        public string Text
        {
            set
            {
                textUI.text = value;
                
                if (textUI.text.Length > 0)
                {
                    background.enabled = true;
                }
            }   
        }

        // public string Text { set { StartCoroutine(SetTextCoroutine(value)); } }

        // private IEnumerator SetTextCoroutine(string text)
        // {
        //     yield return new WaitForSeconds(onsetDelay);
            
        //     textUI.text = text;
                
        //     if (textUI.text.Length > 0)
        //     {
        //         background.enabled = true;
        //     }
        // }

        public void Reset()
        {
            background.enabled = false;
            textUI.text = default(string);
        }

        // public void OnPointerEvent(PointerEventHandler.Event evt, PointerEventData eventData)
        // {
        //     switch (evt)
        //     {
        //         case PointerEventHandler.Event.Enter:
        //             eventData.
        //             break;

        //         case PointerEventHandler.Event.Exit:
        //             break;
        //     }
        // }

        // public override void OnText(string text) => Text = text;
    }
}