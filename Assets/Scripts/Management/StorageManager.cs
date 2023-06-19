using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static HashSet<Vector3> usedSpaces;
    public static HashSet<Vector3> storageLocations;
    private static int _spaceLeft;

    public static List<Resource> resourceList;

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
    
    public static void AddToStorage(Resource resourceToAdd)
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
        Debug.Log(resourceList[0].itemSO.name + " : " + resourceList[0].amount);
    }
    
    public static void UpdateStorage() => _spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => _spaceLeft > 0;
}
