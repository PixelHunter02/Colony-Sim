using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraMovement : MonoBehaviour
{

    public GameObject followObject;
    
    //Movement
    private Vector3 _moveDirection;
    private bool _cameraPanning;
    private Vector3 _mousePosition;
    private Vector2 _lastMousePosition;
    [SerializeField] private float speed = 8f;
    
    //Rotate
    private bool _rightClickPressed;
    private Vector2 _lastRightClickPosition;
    [SerializeField] private float rotationSensitivity;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private Vector3 velocity = Vector3.zero;

    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;
    private Vector3 _offset;

    private void Awake()
    {
        _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
        _cinemachineVirtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _offset = _cinemachineCameraOffset.m_Offset;
    }

    private void Update()
    {
        MoveCameraKeyboard();
    }

    private void FixedUpdate()
    {
        RotateCamera();
        CameraPanningMouse();
    }

    public void BeginMoveCameraKeyboard(InputAction.CallbackContext context)
    {
        var inputDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        _moveDirection = followObject.transform.forward * inputDirection.z + followObject.transform.right * inputDirection.x;
    }

    private void MoveCameraKeyboard()
    {
        followObject.transform.position += _moveDirection * (Time.deltaTime * speed); 
    }
    
    public void BeginPan(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _cameraPanning = true;
            _lastMousePosition = Mouse.current.position.ReadValue();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _cameraPanning = false;
            _moveDirection = Vector3.zero;
        }
    }

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
    }

    private void RotateCamera()
    {
        if(_rightClickPressed && Mouse.current.position.x.ReadValue() > 0)
        {
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
        }
    }
}
