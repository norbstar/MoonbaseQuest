using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityButton = UnityEngine.UI.Button;

namespace Chess
{
    public class AlternatingButtonPanelUIManager : BaseButtonGroupPanelUIManager
    {
        [Header("Components")]
        [SerializeField] UnityButton offButton;
        [SerializeField] UnityButton onButton;
        [SerializeField] SpriteRenderer iconRenderer;

        protected override List<UnityButton> ResolveButtons() => new List<UnityButton>() { offButton, onButton };

        public override void OnClickButton(UnityButton button)
        {
            base.OnClickButton(button);

            var name = button.name;

            if (name.Equals("Off Button"))
            {   
                StartCoroutine(SwitchOnCoroutine());
            }
            else if (name.Equals("On Button"))
            {
                StartCoroutine(SwitchOffCoroutine());
            }
        }

        private IEnumerator SwitchOnCoroutine()
        {
            Debug.Log("On 1");
            offButton.gameObject.SetActive(false);

            var animator = iconRenderer.GetComponent<Animator>() as Animator;
            iconRenderer.gameObject.SetActive(true);
            animator.SetTrigger("Switch On");
            
            AnimatorStateInfo animStateInfo;

            do
            {              
                animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }  
            while (animStateInfo.normalizedTime < 1.0f);

            iconRenderer.gameObject.SetActive(false);
            onButton.gameObject.SetActive(true);

            Debug.Log("On 2");
        }

        private IEnumerator SwitchOffCoroutine()
        {
            Debug.Log("Off 1");
            onButton.gameObject.SetActive(false);

            var animator = iconRenderer.GetComponent<Animator>() as Animator;
            iconRenderer.gameObject.SetActive(true);
            animator.SetTrigger("Switch Off");
            
            AnimatorStateInfo animStateInfo;

            do
            {              
                animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }  
            while (animStateInfo.normalizedTime < 1.0f);

            iconRenderer.gameObject.SetActive(false);
            offButton.gameObject.SetActive(true);

            Debug.Log("Off 2");
        }
    }
}