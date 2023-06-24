using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
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
            _gameManager.cameraMovement.CameraState = CameraState.BuildMode;
            return;
        }
        Instantiate(craftableSo.prefab);
    }
}
