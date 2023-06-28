using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private GameManager _gameManager;
    [SerializeField] private Grid _grid;
    
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    private void Update()
    {
        switch (_gameManager.inputManager.InputMode)
        {
            case InputMode.BuildMode:
                var ray = _gameManager.mainCamera.ScreenPointToRay(playerInputActions.UI.Point.ReadValue<Vector2>());
                Physics.Raycast(ray, out var hit, 1000);
                _grid.WorldToCell(hit.point);
                var cell = _grid.WorldToCell(hit.point);
                Debug.Log(_grid.GetCellCenterWorld(cell));
                Debug.DrawLine(new Vector3(cell.x,-2,cell.z),new Vector3(cell.x+1,-2,cell.z+1));
                Debug.DrawLine(new Vector3(cell.x,-2,cell.z),new Vector3(cell.x,-2,cell.z+1));
                Debug.DrawLine(new Vector3(cell.x,-2,cell.z+1),new Vector3(cell.x+1,-2,cell.z));
                Debug.DrawLine(new Vector3(cell.x+1,-2,cell.z+1),new Vector3(cell.x+1,-2,cell.z));
                break;
        }
        
    }

    public void Crafting(CraftableSO craftableSo)
    {
        int matchingItems = 0;
        List<Resource> resourcesToRemove = new List<Resource>();
        List<int> removalAmount = new List<int>();
        foreach (var ownedResource in StorageManager.resourceList)
        {
            foreach (var required in craftableSo.requiredResource)
            {
                if (ownedResource.itemSO.objectName != required.itemSO.objectName || ownedResource.amount < required.amount)
                    continue;
                matchingItems++;
                resourcesToRemove.Add(ownedResource);
                removalAmount.Add(required.amount);
            }
        }
        
        if (matchingItems != craftableSo.requiredResource.Length)
        {
            Debug.Log("You Dont Have The resources.");
            return;
        }

        for(int i = 0; i < resourcesToRemove.Count; i++)
        {
            StorageManager.EmptyStockpileSpaces(removalAmount[i],resourcesToRemove[i]);
            resourcesToRemove[i].amount -= removalAmount[i];
        }
        StorageManager.UpdateStorage();
        _gameManager.storageManager.DrawInventory();

        if (craftableSo.instantlyEnterBuildMode)
        {
            _gameManager.cameraMovement.PlayerState = PlayerState.BuildMode;
            _gameManager.inputManager.itemBeingBuilt = craftableSo;
            _gameManager.inputManager.InputMode = InputMode.BuildMode;
            // _gameManager.
            return;
        }
        Instantiate(craftableSo.prefab);
    }
}
