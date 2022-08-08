using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class SwitchAnimationTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start() => StartCoroutine(SwitchOnCoroutine());

        private IEnumerator SwitchOnCoroutine()
        {
            var animator = GetComponent<Animator>() as Animator;
            animator.SetTrigger("Switch On");
            
            AnimatorStateInfo animStateInfo;

            do
            {              
                animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }  
            while (animStateInfo.normalizedTime < 1.0f);

            StartCoroutine(SwitchOffCoroutine());
        }

        private IEnumerator SwitchOffCoroutine()
        {
            var animator = GetComponent<Animator>() as Animator;
            animator.SetTrigger("Switch Off");
            
            AnimatorStateInfo animStateInfo;

            do
            {              
                animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }  
            while (animStateInfo.normalizedTime < 1.0f);

            StartCoroutine(SwitchOnCoroutine());
        }
    }
}