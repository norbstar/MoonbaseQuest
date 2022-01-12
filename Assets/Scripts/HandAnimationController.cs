using UnityEngine;

public class HandAnimationController : MonoBehaviour
{
    private Animator animator;

    public void SetFloat(string key, float value)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>() as Animator;
        }

        animator.SetFloat(key, value);
    }
}