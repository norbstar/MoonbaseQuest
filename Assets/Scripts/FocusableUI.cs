using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class FocusableUI : ObservableUI
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] Image focusImage;
    [SerializeField] Sprite observableSprite;
    [SerializeField] Sprite focusSprite;

    [SerializeField] float transitionDistance = 1.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}