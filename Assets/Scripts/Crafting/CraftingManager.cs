using System;
using System.Collections;
using System.Collections.Generic;
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
        foreach (var requiredResource in craftableSo.requiredResource)
        {
            if (!StorageManager.resourceList.Contains(requiredResource)) 
                continue;
            Debug.Log(StorageManager.resourceList.IndexOf(requiredResource));
            var index = StorageManager.resourceList.IndexOf(requiredResource);
            if (StorageManager.resourceList[index].amount >= requiredResource.amount)
            {
                Debug.Log("Has Items");
            }
        }
    }
}
