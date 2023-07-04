using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    // Locations for Storage
    public static HashSet<Vector3> usedSpaces;
    public static HashSet<Vector3> storageLocations;
    
    private static int _spaceLeft;

    public static List<Item> itemList;
    public Transform[] inventorySlots;

    private GameManager _gameManager;
    

    private void Awake()
    {
        storageLocations = new HashSet<Vector3>();
        usedSpaces = new HashSet<Vector3>();
        itemList = new List<Item>();
        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        Stockpile.OnCreateStorageCellEvent += UpdateStorage;
        BuildingManager.itemBuilt += RemoveFromStorage;
    }
    
    private void OnDisable()
    {
        Stockpile.OnCreateStorageCellEvent -= UpdateStorage;
        BuildingManager.itemBuilt -= RemoveFromStorage;
    }
    
    // Used To Add An Item To Storage
    public void AddToStorage(Item itemToAdd)
    {
        bool isItemInInventory = false;
        foreach (Item resource in itemList)
        {
            if (resource.itemSO == itemToAdd.itemSO)
            {
                resource.amount++;
                isItemInInventory = true;
            }
        }

        if (!isItemInInventory || itemList.Count == 0) 
        {
            itemList.Add(itemToAdd);
        }

        //Display Image and Count in Inventory
        DrawInventory();
    }

    public void DrawInventory()
    {
        // Add To Inventory Display
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].amount > 0)
            {
                var image = inventorySlots[i].GetChild(0);
                var count = image.GetChild(0);
                var resource = itemList[i];
                image.gameObject.SetActive(true);
                count.gameObject.SetActive(true);
                image.GetComponent<Image>().sprite = itemList[i].itemSO.uiSprite;
                count.GetComponent<TMP_Text>().text = itemList[i].amount.ToString();
                var button = inventorySlots[i].GetComponent<Button>();
                button.onClick.AddListener(() => _gameManager.buildingManager.currentBuilding = resource.itemSO);
            }
            else
            {
                var image = inventorySlots[i].GetChild(0);
                var count = image.GetChild(0);
                image.gameObject.SetActive(false);
                count.gameObject.SetActive(false);
                itemList.RemoveAt(i);
            }
        }
    }

    public static void EmptyStockpileSpaces(int spacesToClear, Item itemType)
    {
        var storedItemsInformation = FindObjectsOfType<ObjectInformation>();
        int clearedSpaces = 0;
        for (int i = 0; i < storedItemsInformation.Length; i++)
        {
            if (storedItemsInformation[i].Item.objectName == itemType.itemSO.objectName && clearedSpaces < spacesToClear)
            {
                // Debug.Log("ClearingSpaces");
                clearedSpaces++;
                storageLocations.Add(storedItemsInformation[i].storageLocation);
                usedSpaces.Remove(storedItemsInformation[i].storageLocation);
                storedItemsInformation[i].storageLocation = Vector3.zero;
                storedItemsInformation[i]._isStored = false;
                storedItemsInformation[i].gameObject.SetActive(false);
            }
        }
    }

    public static void UseStorageSpace(Vector3 location)
    {
        storageLocations.Remove(location);
        usedSpaces.Add(location);
        UpdateStorage();
    }

    private void RemoveFromStorage(StoredItemSO itemToRemove)
    {
        EmptyStockpileSpaces(1, new Item(){amount = 1, itemSO = itemToRemove});
    }
    
    public static void UpdateStorage() => _spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => _spaceLeft > 0;
}
