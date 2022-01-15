using UnityEngine;

public class IdentityManager : FocusableManager
{
    [SerializeField] IdentityCanvasManager identityCanvasManager;

    void Awake()
    {
        identityCanvasManager.IdentityText = gameObject.transform.parent.name;
    }

    protected override void OnFocusGained()
    {
        identityCanvasManager.gameObject.SetActive(true);
    }

    protected override void OnFocusLost()
    {
        identityCanvasManager.gameObject.SetActive(false);
    }
}