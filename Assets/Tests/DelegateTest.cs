using UnityEngine;

public class DelegateTest : MonoBehaviour
{
    public delegate void Notify(int value);
    public Notify notify;
    public static event Notify EventHandler;

    void OnEnable()
    {
        notify += NotifyReceiver;
        notify += OtherNotifyReceiver;
    }

    void OnDisable()
    {
        notify -= NotifyReceiver;
        notify -= OtherNotifyReceiver;
    }

    // Update is called once per frame
    void Update()
    {
        // NotifyReceiver(5);

        // OR (pass delegate as argument of a function)

        PostNotification(NotifyReceiver, 5);
        PostNotification(OtherNotifyReceiver, 12);

        EventHandler(3);
    }

    private void PostNotification(Notify notify, int value)
    {
        // notify.Invoke(value);

        // OR (shorthand version)

        notify(value);
    }

    private void NotifyReceiver(int value)
    {
        Debug.Log($"Inside Notify Receiver : Value [{value}]");
    }

    private void OtherNotifyReceiver(int value)
    {
        Debug.Log($"Inside Other Notify Receiver : Value [{value}]");
    }
}