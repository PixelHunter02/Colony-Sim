using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private GameManager _gameManager;
    [SerializeField] private GameObject test;

    [SerializeField] private CraftableSO[] craftingRecipes;

    public CraftableSO[] CraftingRecipes => craftingRecipes;


    private void Awake()
    {
        _gameManager = GameManager.Instance;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    public void Crafting(CraftableSO craftableSo)
    {
        int matchingItems = 0;
        List<Item> resourcesToRemove = new List<Item>();
        List<int> removalAmount = new List<int>();
        foreach (var ownedResource in StorageManager.itemList)
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
        _gameManager.storageManager.AddToStorage(new Item(){amount = 1,itemSO = craftableSo.itemToStore});
        StorageManager.UpdateStorage();
        _gameManager.storageManager.DrawInventory();
    }
}
