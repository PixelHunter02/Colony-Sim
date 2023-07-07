using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;
    
    private bool isRotating;
    private bool isPanning;

    //Cameras
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera buildCamera;
    
    private InputMode _inputMode;
    
    public InputMode InputMode
    {
        get => _inputMode;
        set
        {
            _inputMode = value;
            
            switch (_inputMode)
            {
                case InputMode.DefaultMode:
                    mainCamera.gameObject.SetActive(true);
                    buildCamera.gameObject.SetActive(false);
                    break;
                case InputMode.BuildMode:
                    mainCamera.gameObject.SetActive(false);
                    buildCamera.gameObject.SetActive(true);
                    break;
                case InputMode.Stockpile:
                    break;
            }
        }
    }

    public CraftableSO itemBeingBuilt;
    
    private GameManager _gameManager;

    [SerializeField] private float interactDistance = 25;

    [SerializeField] private LayerMask _layerMask;

    private void Awake()
    {
        // Activate Input Actions
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); 
        playerInputActions.UI.Enable(); 
        
        // Get reference to Game Manager
        _gameManager = GameManager.Instance;
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

    public bool EscapePressed()
    {
        if (playerInputActions.Player.Escape.ReadValue<float>() > 0)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetMouseToWorldPosition()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(playerInputActions.UI.Point.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out var hit, 1000, _layerMask) && !_gameManager.IsOverUI()) 
            return hit.point;

        return Vector3.zero;
    }
    
    public Vector3 GetMouseToWorldPositionCursor()
    {
        var ray = _gameManager.mainCamera.ScreenPointToRay(playerInputActions.UI.Point.ReadValue<Vector2>());
        // if(Physics.Raycast(ray, out var hitObj, Mathf.Infinity,~3))
        if (Physics.Raycast(ray, out var hit, interactDistance,_layerMask) && !_gameManager.IsOverUI()) 
            return hit.point;

        return Vector3.zero;
    }
}

public enum InputMode
{
    DefaultMode,
    BuildMode,
    Stockpile,
}
