using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class CameraMovement : MonoBehaviour
{

    public GameObject followObject;
    [SerializeField] private float speed = 5f;
    //Movement
    private Vector3 moveDirection;

    private void Update()
    {
        MoveCamera();
    }

    public void MoveCamera()
    {
        followObject.transform.position += moveDirection * Time.deltaTime * speed; 
        Debug.Log(moveDirection);
    }

    public void BeginMoveCamera(InputAction.CallbackContext context)
    {
        moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }
}
