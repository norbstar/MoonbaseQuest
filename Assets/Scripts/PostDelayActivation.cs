using System.Collections;

using UnityEngine;

public class PostDelayActivation : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject target;
    [SerializeField] float delaySec = 1f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(ActivatePostDelayCoroutine(delaySec));
    }

    private IEnumerator ActivatePostDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delaySec);
        target.SetActive(true);
    }
}