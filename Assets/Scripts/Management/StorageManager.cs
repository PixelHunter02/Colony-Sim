using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static HashSet<Vector3> usedSpaces;
    public static HashSet<Vector3> storageLocations;
    private static int spaceLeft;


    private void Awake()
    {
        storageLocations = new HashSet<Vector3>();
        usedSpaces = new HashSet<Vector3>();
    }

    private void OnEnable()
    {
        Stockpile.OnCreateStorageCellEvent += UpdateStorage;
    }
    
    public void StoreItem(PickupObject itemToStore)
    {
        if(!HasStorageSpace())
            return;
        var location = storageLocations.ElementAt(0);
        itemToStore.TryGetComponent(out PickupObject pickupObject);
        pickupObject.storedAt = location;
        storageLocations.Remove(location);
        itemToStore.transform.rotation = Quaternion.Euler(0,0,0);
        usedSpaces.Add(location);
        UpdateStorage();
    }
    private void UpdateStorage() => spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => spaceLeft > 0;
}
