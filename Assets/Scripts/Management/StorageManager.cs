using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    public static HashSet<Vector3> usedSpaces;
    public static HashSet<Vector3> storageLocations;
    private static int _spaceLeft;

    public static List<Resource> resourceList;
    public Transform[] inventorySlots;
    

    private void Awake()
    {
        storageLocations = new HashSet<Vector3>();
        usedSpaces = new HashSet<Vector3>();
        resourceList = new List<Resource>();
    }

    private void OnEnable()
    {
        Stockpile.OnCreateStorageCellEvent += UpdateStorage;
    }
    
    public void AddToStorage(Resource resourceToAdd)
    {
        bool isItemInInventory = false;
        foreach (Resource resource in resourceList)
        {
            if (resource.itemSO == resourceToAdd.itemSO)
            {
                resource.amount++;
                isItemInInventory = true;
            }
        }

        if (!isItemInInventory || resourceList.Count == 0) 
        {
            resourceList.Add(resourceToAdd);
        }

        //Display Image and Count in INventory
        DrawInventory();
    }

    public void DrawInventory()
    {
        // Add To Inventory Display
        for (int i = 0; i < resourceList.Count; i++)
        {
            if (resourceList[i].amount > 0)
            {
                var image = inventorySlots[i].GetChild(0);
                var count = image.GetChild(0);
                image.gameObject.SetActive(true);
                count.gameObject.SetActive(true);
                image.GetComponent<Image>().sprite = resourceList[i].itemSO.uiSprite;
                count.GetComponent<TMP_Text>().text = resourceList[i].amount.ToString();
            }
            else
            {
                var image = inventorySlots[i].GetChild(0);
                var count = image.GetChild(0);
                image.gameObject.SetActive(false);
                count.gameObject.SetActive(false);
                resourceList.RemoveAt(i);
            }
        }
    }

    public static void EmptyStockpileSpaces(int spacesToClear, Resource resourceType)
    {
        var storedItemsInformation = FindObjectsOfType<ObjectInformation>();
        int clearedSpaces = 0;
        for (int i = 0; i < storedItemsInformation.Length; i++)
        {
            if (storedItemsInformation[i].Item == resourceType.itemSO && clearedSpaces < spacesToClear)
            {
                clearedSpaces++;
                storageLocations.Add(storedItemsInformation[i].storageLocation);
                usedSpaces.Remove(storedItemsInformation[i].storageLocation);
                storedItemsInformation[i].storageLocation = Vector3.zero;
                storedItemsInformation[i]._isStored = false;
                storedItemsInformation[i].gameObject.SetActive(false);
            }
        }
    }
    
    public static void UpdateStorage() => _spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => _spaceLeft > 0;
}
