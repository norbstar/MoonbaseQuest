using UnityEngine;

// https://docs.unity3d.com/ScriptReference/ExecuteAlways.html

[ExecuteAlways]
public class ExecuteAlwaysTest : MonoBehaviour
{
    [SerializeField] int iteration;
    
    // Start is called before the first frame update
    void Start()
    {

        if (Application.IsPlaying(gameObject))
        {
            // Play logic
            iteration = 0;
        }
        else
        {
            // Editor logic
            iteration = 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.IsPlaying(gameObject))
        {
            // Play logic
            iteration += 5;
        }
        else
        {
            // Editor logic
            iteration += 1;
        }       
    }
}