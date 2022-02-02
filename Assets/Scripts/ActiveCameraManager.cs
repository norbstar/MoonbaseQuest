using UnityEngine;

public class ActiveCameraManager : MonoBehaviour {
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera _3rdPersonCamera;
    [SerializeField] Camera overheadCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        EnableMainView();
    }

    [ContextMenu("Enable Main View")]
    public void EnableMainView()
    {
        mainCamera.enabled = true;
        _3rdPersonCamera.enabled = false;
        overheadCamera.enabled = false;
    }

    [ContextMenu("Enable 3rd Person View")]
    public void EnableThirdPersonView()
    {
        mainCamera.enabled = false;
        _3rdPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }

    [ContextMenu("Enable Overhead View")]
    public void EnableOverheadView()
    {
        mainCamera.enabled = false;
        _3rdPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }
}