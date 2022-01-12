using UnityEngine;

public class DelegateClientTest : MonoBehaviour
{
    void OnEnable()
    {
        DelegateTest.EventHandler += EventReceiver;
        DelegateTest.EventHandler += OtherEventReceiver;
    }

    void OnDisable()
    {
        DelegateTest.EventHandler -= EventReceiver;
        DelegateTest.EventHandler -= OtherEventReceiver;
    }

    private void EventReceiver(int value)
    {
        Debug.Log($"Inside Event Receiver : Value [{value}]");
    }

    private void OtherEventReceiver(int value)
    {
        Debug.Log($"Inside Other Event Receiver : Value [{value}]");
    }
}