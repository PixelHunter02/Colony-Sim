using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class CameraMovement : MonoBehaviour
{
    //Managers
    private GameManager _gameManager;

    //Focus Object
    public GameObject followObject;
    
    //Movement
    private Vector2 _lastMousePosition;
    [SerializeField] private float keyboardMoveSpeed = 30;
    [SerializeField] private float panningSpeed;
    
    //Rotate
    private Vector2 _lastRightClickPosition;

    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;
    private CinemachineVirtualCamera _cinemachineVCam;

    public GameObject cursorGO;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AssignValues;
    
        EnhancedTouchSupport.Enable();
    }

    private void Start()
    {
        _gameManager.inputManager.playerInputActions.Player.CameraPanning.performed += StartCameraPanning;
        _gameManager.inputManager.playerInputActions.Player.CameraPanning.canceled += _ => EndCameraPanning();    
        _gameManager.inputManager.playerInputActions.Player.RotateCamera.performed += _ => StartRotateCamera();
        _gameManager.inputManager.playerInputActions.Player.RotateCamera.canceled += _ => EndRotateCamera(); 
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("GameScene") || SceneManager.GetActiveScene().name.Equals("Tablet"))
        {
            CameraZoom();
            MoveCamera();
            MoveCursor();
        }
    }

    private void MoveCamera()
    {
        var movementDirection = new Vector3(_gameManager.inputManager.GetNormalizedMovement().x, 0, _gameManager.inputManager.GetNormalizedMovement().y);

        movementDirection = followObject.transform.forward * movementDirection.z + followObject.transform.right * movementDirection.x;
        followObject.transform.position += movementDirection * (keyboardMoveSpeed * Time.deltaTime * SettingsManager.keyboardMoveSpeedModifier);
    }

    private void StartCameraPanning(InputAction.CallbackContext context)
    {
        if(_lastMousePosition == Vector2.zero )
        {
            var position = _gameManager.inputManager.playerInputActions.Player.secondFinger.ReadValue<Vector2>();
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame(position);
            panning = StartCoroutine(OnPanningCR());
        }
    }

    private Coroutine panning;
    private IEnumerator OnPanningCR()
    {
        while (true)
        {
            var position = _gameManager.inputManager.playerInputActions.Player.secondFinger.ReadValue<Vector2>();

            var mousePosition = followObject.transform.forward * (_gameManager.inputManager.GetScaledCursorPositionThisFrame(position).y - _lastMousePosition.y) + followObject.transform.right*(_gameManager.inputManager.GetScaledCursorPositionThisFrame(position).x-_lastMousePosition.x);
            var movementDirection = new Vector3(mousePosition.x, 0, mousePosition.z);
            followObject.transform.position += movementDirection * (Time.deltaTime * panningSpeed * SettingsManager.mousePanSpeedModifier);
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame(position);

            // if (_gameManager.IsOverUI())
            // {
            //     yield break;
            // }
            yield return null;
        }
    }

    private void EndCameraPanning()
    {
        _lastMousePosition = Vector2.zero;
        StopCoroutine(panning);
    }

    /// <summary>
    /// PC
    /// </summary>
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

    private void StartRotateCamera()
    {
        Debug.Log("Started Rotating");
        if(_lastMousePosition == Vector2.zero )
        {
            var position = _gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>();
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame(position);
            rotateCamera = StartCoroutine(OnRotateCamera());
        }
        
    }

    public Coroutine rotateCamera;

    private IEnumerator OnRotateCamera()
    {
        Debug.Log("Started Rotate");
        while (true)
        {
            var position = _gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>();

            var value = _gameManager.inputManager.GetScaledCursorPositionThisFrame(position) - _lastMousePosition;
            var rotation = followObject.transform.rotation.eulerAngles;
            rotation.y += value.x * SettingsManager.rotationSpeedModifier * _gameManager.settingsManager.CameraXModifier() * Time.deltaTime * 5;
            followObject.transform.rotation = Quaternion.Euler(rotation);
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame(position);
            yield return null;
        }
    }

    private void EndRotateCamera()
    {
        _lastMousePosition = Vector2.zero;
        StopCoroutine(rotateCamera);
    }

    private void MoveCursor()
    {
        // var cursorPosition = ;
        if (_gameManager.inputManager.GetMouseToWorldPositionCursor() != Vector3.zero)
        {
            cursorGO.SetActive(true);
            var cell = _gameManager.inputManager.GetMouseToWorldPositionCursor();
            var cellPos = _gameManager.grid.GetCellCenterWorld(Vector3Int.FloorToInt(cell));
            cursorGO.transform.position= new Vector3(cellPos.x, cellPos.y-0.48f, cellPos.z); 
        }
        else
        {
            cursorGO.SetActive(false);
        }
    }

    private void AssignValues(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("GameScene"))
        {
            _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
            _cinemachineVCam = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();  
            followObject = GameObject.Find("Follow Object");
        }
    }
}

