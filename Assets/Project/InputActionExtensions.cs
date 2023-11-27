using UnityEngine.InputSystem;

public static class InputActionExtensions
{
    // public static bool IsPressed(this InputAction action) => action.ReadValue<float>() > 0f;
 
    public static bool WasPressedThisFrame(this InputAction action) => action.triggered && action.ReadValue<float>() > 0f;
 
    public static bool WasReleasedThisFrame(this InputAction action) => action.triggered && action.ReadValue<float>() == 0f;
}