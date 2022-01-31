using UnityEngine;

public class ActiveCameraManager : MonoBehaviour {
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera _3rdPersonCamera;
    [SerializeField] Camera overheadCamera;
    
    [ContextMenu("Show Main View")]
    public void ShowMainView() {
        mainCamera.enabled = true;
        _3rdPersonCamera.enabled = false;
        overheadCamera.enabled = false;
    }

    [ContextMenu("Show 3rd Person View")]
    public void ShowThirdPersonView() {
        mainCamera.enabled = false;
        _3rdPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }

    [ContextMenu("Show Overhead View")]
    public void ShowOverheadView() {
        mainCamera.enabled = false;
        _3rdPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }
}