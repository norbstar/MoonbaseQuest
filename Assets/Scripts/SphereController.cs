using System.Collections;

using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] float speed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
            yield return null;
        }
    }
}