using UnityEngine;

[RequireComponent(typeof(UnitLogger))]
public class UnitTestDemo : MonoBehaviour
{
    private UnitLogger unitLogger;
    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => unitLogger = GetComponent<UnitLogger>() as UnitLogger;

    // Start is called before the first frame update
    void Start() => unitLogger.Log("Hello World!", new UnitLogger.Prefix { text = "Message", color = Color.green }, gameObject);       
}