using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class Interactions : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
 
    [SerializeField] private Camera cam;

    [SerializeField] private List<Worker> workers;
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        foreach (var villager in GameObject.FindGameObjectsWithTag("Villagers"))
        {
            villager.TryGetComponent(out Worker worker);
            workers.Add(worker);
        }
    }

    private void Start()
    {
        _playerInputActions.Player.Select.performed += InteractObject;
    }

    private void InteractObject(InputAction.CallbackContext context)
    {
        // Get the information of the object being clicked
        var ray = cam.ScreenPointToRay(_playerInputActions.UI.Point.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out var hit, 100)) 
            return;
        
        // Check if the clicked object has the component ObjectManager
        hit.transform.TryGetComponent(out HarvestObjectManager objectManager);
        if (!objectManager) 
            return;
        
        // Run through each worker for an available worker who is of the correct role.
        foreach (var worker in workers)
        {
            if (!objectManager.harvestableObject.canInteract.Contains(worker.role) || worker.interactingWith != null || objectManager.assignedWorker != null) 
                continue;
            
            BeginWorking(worker, objectManager);
            worker.StartCoroutine(worker.MoveToJob(worker, objectManager));
            break;
        }
    }

    private static void BeginWorking(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        harvestObjectManager.assignedWorker = worker;
        worker.interactingWith = harvestObjectManager;
    }
}
