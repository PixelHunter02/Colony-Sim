using System;
using Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Managers
    private GameManager _gameManager;

    //Focus Object
    public GameObject followObject;
    
    //Movement
    private Vector2 _lastMousePosition;
    [SerializeField] private float speed;
    [SerializeField] private float panningSpeed;
    
    //Rotate
    private Vector2 _lastRightClickPosition;

    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;
    private CinemachineVirtualCamera _cinemachineVCam;
    
    //Cameras
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera buildCamera;

    //Camera State
    private PlayerState _playerState;
    // Setter Allows The State Change To Run Whatever Is Under it (similar To An Event)
    public PlayerState PlayerState
    {
        get => _playerState;
        set
        {
            _playerState = value;
            switch (_playerState)
            {
                case PlayerState.BuildMode:
                    mainCamera.gameObject.SetActive(false);
                    buildCamera.gameObject.SetActive(true);
                    break;
                case PlayerState.MainCamera:
                    mainCamera.gameObject.SetActive(true);
                    buildCamera.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    
    private void Awake()
    {
        _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _cinemachineVCam = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            CameraZoom();
        }
        
        MoveCamera();
        CameraPanning();
        RotateCamera();
    }

    private void MoveCamera()
    {
        var movementDirection = new Vector3(_gameManager.inputManager.GetNormalizedMovement().x, 0, _gameManager.inputManager.GetNormalizedMovement().y);

        movementDirection = followObject.transform.forward * movementDirection.z + followObject.transform.right * movementDirection.x;
        followObject.transform.position += movementDirection * (speed * Time.deltaTime);
    }

    private void CameraPanning()
    {
        if(!_gameManager.inputManager.IsPanning())
        {
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame();
        }

        if(_gameManager.inputManager.IsPanning())
        {
            var mousePosition = followObject.transform.forward * (_gameManager.inputManager.GetScaledCursorPositionThisFrame().y - _lastMousePosition.y) + followObject.transform.right*(_gameManager.inputManager.GetScaledCursorPositionThisFrame().x-_lastMousePosition.x);
            var movementDirection = new Vector3(mousePosition.x, 0, mousePosition.z);
            followObject.transform.position += movementDirection * (Time.deltaTime * panningSpeed);
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame();
        }
    }
    
    private void CameraZoom()
    {
        var zoomValue = _gameManager.inputManager.ZoomValueAsInt();
        const int minimumZoomValue = -5;
        const int maximumZoomValue = 0;
        _cinemachineCameraOffset.m_Offset.z += zoomValue;
        _cinemachineCameraOffset.m_Offset.z =
            Mathf.Clamp(_cinemachineCameraOffset.m_Offset.z, minimumZoomValue, maximumZoomValue);
        var transposerOffset = _cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        transposerOffset += zoomValue * -2;
        _cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(transposerOffset, 3, 15);
    }

    private void RotateCamera()
    {
        if(!_gameManager.inputManager.IsRotating())
        {
            _lastRightClickPosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame();
            return;
        }
        
        var value = _gameManager.inputManager.GetScaledCursorPositionThisFrame() - _lastRightClickPosition;
        var rotation = followObject.transform.rotation.eulerAngles;
        rotation.y += value.x * _gameManager.settingsManager.rotationSpeed * _gameManager.settingsManager.CameraXModifier() * Time.deltaTime;
        followObject.transform.rotation = Quaternion.Euler(rotation);
        _lastRightClickPosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame();
    }
}

public enum PlayerState
{
    MainCamera,
    BuildMode,
}
