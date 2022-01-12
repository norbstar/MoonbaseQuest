using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(IKeyDown))]
public class KeyboardInput : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigger;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onTrigger.Invoke();
        }
    }
}