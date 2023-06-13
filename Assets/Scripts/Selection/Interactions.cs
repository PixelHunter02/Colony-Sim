using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactions : MonoBehaviour
{
    
    private PlayerInputActions _playerInputActions;
    [SerializeField] private Camera cam;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        _playerInputActions.Player.Select.performed += PickUpSelected;
    }

    private void PickUpSelected(InputAction.CallbackContext context)
    {
        Ray ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log(hit.transform.name);
        }
    }
}
