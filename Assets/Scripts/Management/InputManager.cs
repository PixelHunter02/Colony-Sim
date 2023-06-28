using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private bool isRotating;
    private bool isPanning;

    private InputMode _inputMode;

    public InputMode InputMode
    {
        get => _inputMode;
        set
        {
            _inputMode = value;
        }
    }

    public CraftableSO itemBeingBuilt;
    
    private GameManager _gameManager;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); 
        playerInputActions.UI.Enable(); 
        
        // Get reference to Game Manager
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        switch (_inputMode)
        {
            case InputMode.DefaultMode:
                break;
            case InputMode.BuildMode:
                Building();
                break;
            case InputMode.Stockpile:
                break;
        }
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
        
        Debug.Log(scrollValue);
       
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

    public bool EscapePressed()
    {
        if (playerInputActions.Player.Escape.ReadValue<float>() > 0)
        {
            return true;
        }
        return false;
    }

    private void Building()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 1000) || _gameManager.uiManager.IsOverUI() ||!_gameManager.uiManager.stockpileMode) 
            return;

        var placementPosition = hit.point;
    }
}

public enum InputMode
{
    BuildMode,
    Stockpile,
    DefaultMode,
}
