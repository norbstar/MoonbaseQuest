using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ScrollView))]
public class ScrollViewManager : MonoBehaviour
{
    private ScrollView scrollView;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        scrollView = GetComponent<ScrollView>() as ScrollView;
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO
    }
}
