using System.Collections;

using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Assets")]
    public GameObject arrowPrefab = null;

    [Header("Bow")]
    public float grabThreshold = 0.15f;
    public Transform start = null;
    public Transform end = null;
    public Transform socket = null;

    private Transform pullingHand = null;
    private Arrow currentArrow = null;
    private Animator animator = null;
    private float pullValue = 0f;

    private void Awake() => animator = GetComponent<Animator>();

    private void Start () => Co_CreateArrow(0f);

    private IEnumerator Co_CreateArrow(float waitTime)
    {
        // Wait
        yield return new WaitForSeconds(waitTime);

        // Create instance
        GameObject arrow = Instantiate(arrowPrefab, socket);

        // Orientate
        arrow.transform.localPosition = new Vector3(0f, 0f, 0.425f);
        arrow.transform.localEulerAngles = Vector3.zero;

        // Set
        currentArrow = arrow.GetComponent<Arrow>();
    }

    public void Pull(Transform hand)
    {

    }

    public void Release()
    {
        
    }
}
