using System.IO;

using UnityEngine;

using TMPro;

public class DebugCanvasUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public TextMeshProUGUI textUI;
    
    // Start is called before the first frame update
    // void Start()
    // {
    //     textUI.text = Path.Combine(Application.persistentDataPath, $"file.log");
    // }
}