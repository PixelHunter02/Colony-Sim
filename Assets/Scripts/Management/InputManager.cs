using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private bool isRotating;
    private bool isPanning;
    
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); 
        playerInputActions.UI.Enable(); 
    }

    public Vector2 GetNormalizedMovement()
    {
        var inputDirection = playerInputActions.Player.MoveCamera.ReadValue<Vector2>();

        var normalizedValue = inputDirection.normalized;
        return normalizedValue;
    }

    public int ZoomValueAsInt()
    {
        var inputValue = playerInputActions.Player.CameraZoom.ReadValue<Vector2>();

        var scrollValue = inputValue.y switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0,
        };
       
        return scrollValue;
    }

    public Vector2 GetScaledCursorPositionThisFrame()
    {
        var position = playerInputActions.UI.Point.ReadValue<Vector2>();
        var scaledPosition = new Vector2(position.x / Screen.width * 1920, position.y / Screen.height * 1080);
        return scaledPosition;
    }

    public bool IsPanning()
    {
        var phase = playerInputActions.Player.CameraPanning.phase;
        isPanning = phase is InputActionPhase.Started or InputActionPhase.Performed;

        return isPanning;
    }

    public bool IsRotating()
    {
        var phase = playerInputActions.Player.RotateCamera.phase;
        isRotating = phase is InputActionPhase.Started or InputActionPhase.Performed;

        return isRotating;
    }
}
