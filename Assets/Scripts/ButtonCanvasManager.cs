using UnityEngine;

public class ButtonCanvasManager : BaseManager
{
    public void OnExit()
    {
        Log($"{gameObject.name}.OnExit");
    }
}