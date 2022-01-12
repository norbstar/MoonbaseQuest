using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Canvas))]
public class GunAmmoCanvasManager : MonoBehaviour
{
    [SerializeField] int defaultLoadout = 16;
    [SerializeField] TextMeshProUGUI ammoTextUI;
    [SerializeField] Image modeUI;
    
    [Header("Sprites")]
    [SerializeField] Sprite singleShot;
    [SerializeField] Sprite multiShot;

    public int AmmoCount {
        get
        {
            return ammoCount;
        }
        
        set
        {
            ammoTextUI.text = String.Format("{0:00}", value);
            ammoTextUI.color = (value > 0) ? Color.white : Color.red;
        }
    }

    private int ammoCount;

    // Start is called before the first frame update
    void Start()
    {
        ammoCount = defaultLoadout;
        AmmoCount = ammoCount;
    }

    public void RestoreAmmoCount()
    {
        ammoCount = defaultLoadout;
        AmmoCount = ammoCount;
    }

    public void DecrementAmmoCount()
    {
        ammoCount -= 1;
        AmmoCount = ammoCount;
    }

    public void SetMode(GunInteractableManager.Mode mode)
    {
        switch (mode)
        {
            case GunInteractableManager.Mode.Manual:
                modeUI.sprite = singleShot;
                break;

            case GunInteractableManager.Mode.Auto:
                modeUI.sprite = multiShot;
                break;
        }
    }
}