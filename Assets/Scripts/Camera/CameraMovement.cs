using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject cursorGO;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AssignValues;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("New Scene"))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                CameraZoom();
            }

            MoveCamera();
            CameraPanning();
            RotateCamera();
            MoveCursor();
        }
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
        if (scene.name.Equals("New Scene"))
        {
            _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
            _cinemachineVCam = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();  
            followObject = GameObject.Find("Follow Object");
        }
    }
}

public enum PlayerState
{
    MainCamera,
    BuildMode,
}
