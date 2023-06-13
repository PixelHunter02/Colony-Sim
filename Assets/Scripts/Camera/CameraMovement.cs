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
<<<<<<< Updated upstream
    [SerializeField] private float rotationSensitivity;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private Vector3 velocity = Vector3.zero;

=======
    
>>>>>>> Stashed changes
    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;


    private void Awake()
    {
        _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
    }

    private void Update()
    {
<<<<<<< Updated upstream
        MoveCameraKeyboard();
    }

    private void FixedUpdate()
    {
=======
        CameraZoom();
        MoveCamera();
        CameraPanning();
>>>>>>> Stashed changes
        RotateCamera();
        CameraPanningMouse();
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
        }
    }
    
    private void CameraZoom()
    {
        var zoomValue = inputManager.ZoomValueAsInt();

<<<<<<< Updated upstream
    public void CameraPanningMouse()
    {
        if (_cameraPanning)
        {
            _mousePosition = (-followObject.transform.forward * (Mouse.current.position.y.ReadValue() -_lastMousePosition.y)) * (Time.deltaTime*2) + (-followObject.transform.right*(Mouse.current.position.x.ReadValue()-_lastMousePosition.x)) * (Time.deltaTime*2);
            _lastMousePosition = Mouse.current.position.ReadValue();
            _moveDirection = new Vector3(_mousePosition.x, 0, _mousePosition.z) ;
        }
    }

    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (context.ReadValue<Vector2>().y > 0)
            {
                _offset.z += 5;
                _offset.z = Mathf.Clamp(_offset.z, -25, 0);
                StopAllCoroutines();
                StartCoroutine(Zoom());
            }
            else if (context.ReadValue<Vector2>().y < 0)
            {
                _offset.z -= 5;
                _offset.z = Mathf.Clamp(_offset.z, -25, 0);
                StopAllCoroutines();
                StartCoroutine(Zoom());
            }
        }
    }

    private IEnumerator Zoom()
    {
        float progress = 0; 
        while(progress < 1)
        {
            Debug.Log(progress);
            float timeElapsed = Time.deltaTime;
            progress += timeElapsed/4;
            _cinemachineCameraOffset.m_Offset = Vector3.Lerp(_cinemachineCameraOffset.m_Offset, _offset, progress);
            yield return null;
        }
    }

    public void BeginRotateCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _rightClickPressed = true;
            _lastRightClickPosition = Mouse.current.position.ReadValue();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _rightClickPressed = false;
        }
=======
        const int minimumZoomValue = -25;
        const int maximumZoomValue = 0;
        _cinemachineCameraOffset.m_Offset.z += zoomValue * 5;
        _cinemachineCameraOffset.m_Offset.z = Mathf.Clamp(_cinemachineCameraOffset.m_Offset.z, minimumZoomValue, maximumZoomValue);
>>>>>>> Stashed changes
    }

    private void RotateCamera()
    {
        if(!inputManager.IsRotating())
        {
<<<<<<< Updated upstream
            //Left and Right Rotation;
            var value = Mouse.current.position.ReadValue() - _lastRightClickPosition;
            Vector3 rotation = followObject.transform.rotation.eulerAngles;
            rotation.y += value.x;
            rotation = Vector3.SmoothDamp(followObject.transform.rotation.eulerAngles, rotation, ref velocity, 0.25f);
            followObject.transform.rotation = Quaternion.Euler(rotation);
            
            // Up And Down Rotation
            var vcamOffset = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
            var original = vcamOffset;
            vcamOffset.y += -value.y * rotationSensitivity;
            var clamped = Mathf.Clamp(vcamOffset.y, 0, 15);
            _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = clamped;
            
            _lastRightClickPosition = Mouse.current.position.ReadValue();
=======
            _lastRightClickPosition = inputManager.GetScaledCursorPositionThisFrame();
            return;
>>>>>>> Stashed changes
        }
        
        var value = inputManager.GetScaledCursorPositionThisFrame() - _lastRightClickPosition;
        var rotation = followObject.transform.rotation.eulerAngles;
        rotation.y += value.x * settingsManager.rotationSpeed * settingsManager.CameraXModifier() * Time.deltaTime;
        followObject.transform.rotation = Quaternion.Euler(rotation);
    }
}
