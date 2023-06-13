using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Managers
    [SerializeField] private InputManager inputManager;
    [SerializeField] private SettingsManager settingsManager;

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
        var movementDirection = new Vector3(inputManager.GetNormalizedMovement().x, 0, inputManager.GetNormalizedMovement().y);

        movementDirection = followObject.transform.forward * movementDirection.z + followObject.transform.right * movementDirection.x;
        followObject.transform.position += movementDirection * (speed * Time.deltaTime);
    }

    private void CameraPanning()
    {
        if(!inputManager.IsPanning())
        {
            _lastMousePosition = inputManager.GetScaledCursorPositionThisFrame();
        }

        if(inputManager.IsPanning())
        {
            var mousePosition = followObject.transform.forward * (inputManager.GetScaledCursorPositionThisFrame().y - _lastMousePosition.y) + followObject.transform.right*(inputManager.GetScaledCursorPositionThisFrame().x-_lastMousePosition.x);
            var movementDirection = new Vector3(mousePosition.x, 0, mousePosition.z);
            followObject.transform.position += movementDirection * (Time.deltaTime);
            _lastMousePosition = inputManager.GetScaledCursorPositionThisFrame();
        }
    }
    
    private void CameraZoom()
    {
        var zoomValue = inputManager.ZoomValueAsInt();
        const int minimumZoomValue = -25;
        const int maximumZoomValue = 0;
        _cinemachineCameraOffset.m_Offset.z += zoomValue * 5;
        _cinemachineCameraOffset.m_Offset.z = Mathf.Clamp(_cinemachineCameraOffset.m_Offset.z, minimumZoomValue, maximumZoomValue);
    }

    private void RotateCamera()
    {
        if(!inputManager.IsRotating())
        {
            _lastRightClickPosition = inputManager.GetScaledCursorPositionThisFrame();
            return;
        }
        
        var value = inputManager.GetScaledCursorPositionThisFrame() - _lastRightClickPosition;
        var rotation = followObject.transform.rotation.eulerAngles;
        rotation.y += value.x * settingsManager.rotationSpeed * settingsManager.CameraXModifier() * Time.deltaTime;
        followObject.transform.rotation = Quaternion.Euler(rotation);
    }
}
