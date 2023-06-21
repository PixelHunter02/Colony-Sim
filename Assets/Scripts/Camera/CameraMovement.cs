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
    
    //Rotate
    private Vector2 _lastRightClickPosition;

    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;


    private void Awake()
    {
        _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        CameraZoom();
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
            followObject.transform.position += movementDirection * (Time.deltaTime);
            _lastMousePosition = _gameManager.inputManager.GetScaledCursorPositionThisFrame();
        }
    }
    
    private void CameraZoom()
    {
        var zoomValue = _gameManager.inputManager.ZoomValueAsInt();
        const int minimumZoomValue = -25;
        const int maximumZoomValue = 0;
        _cinemachineCameraOffset.m_Offset.z += zoomValue * 5;
        _cinemachineCameraOffset.m_Offset.z = Mathf.Clamp(_cinemachineCameraOffset.m_Offset.z, minimumZoomValue, maximumZoomValue);
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
    }
}
