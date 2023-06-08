using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;


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
    private bool rightClickPressed;
    private Vector2 _lastRightClickPosition;
    [SerializeField] private float rotationSensitivity;
    
    //zoom
    private CinemachineCameraOffset _cinemachineCameraOffset;
    private Vector3 offset;

    private void Awake()
    {
        _cinemachineCameraOffset = GameObject.Find("VirtualCamera").GetComponent<CinemachineCameraOffset>();
        offset = _cinemachineCameraOffset.m_Offset;
    }

    private void Update()
    {
        MoveCameraKeyboard();
        CameraPanningMouse();
    }

    private void FixedUpdate()
    {
        RotateCamera();
    }

    public void BeginMoveCameraKeyboard(InputAction.CallbackContext context)
    {
        var inputDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        _moveDirection = followObject.transform.forward * inputDirection.z + followObject.transform.right * inputDirection.x;
    }

    public void MoveCameraKeyboard()
    {
        followObject.transform.position += _moveDirection * Time.deltaTime * speed; 
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
            Debug.Log(_mousePosition);
            _mousePosition = followObject.transform.forward * (Mouse.current.position.y.ReadValue()-_lastMousePosition.y) + followObject.transform.right*(Mouse.current.position.x.ReadValue()-_lastMousePosition.x);
            _moveDirection = new Vector3(_mousePosition.x, 0, _mousePosition.z)*Time.deltaTime*1.5f;
        }
    }

    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (context.ReadValue<Vector2>().y > 0)
            {
                offset.z += 5;
                StopAllCoroutines();
                StartCoroutine(Zoom());
            }
            else if (context.ReadValue<Vector2>().y < 0)
            {
                offset.z -= 5;
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
            _cinemachineCameraOffset.m_Offset = Vector3.Lerp(_cinemachineCameraOffset.m_Offset, offset, progress);
            yield return null;
        }
    }

    public void BeginRotateCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            rightClickPressed = true;
            _lastRightClickPosition = Mouse.current.position.ReadValue();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            rightClickPressed = false;
        }
    }

    private void RotateCamera()
    {
        if(rightClickPressed && Mouse.current.position.x.ReadValue() > 0)
        {
            var value = Mouse.current.position.ReadValue() - _lastRightClickPosition;
            Vector3 rotation = followObject.transform.rotation.eulerAngles;
            rotation.y += value.x * rotationSensitivity * Time.deltaTime;
            followObject.transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
