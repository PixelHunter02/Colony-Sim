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

    private static List<Resource> resourceList;
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

        for (int i = 0; i < resourceList.Count; i++)
        {
            var image = inventorySlots[i].GetChild(0);
            var count = image.GetChild(0);
            image.gameObject.SetActive(true);
            count.gameObject.SetActive(true);
            image.GetComponent<Image>().sprite = resourceList[i].itemSO.uiSprite;
            count.GetComponent<TMP_Text>().text = resourceList[i].amount.ToString();
        }
    }
    
    public static void UpdateStorage() => _spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => _spaceLeft > 0;
}
