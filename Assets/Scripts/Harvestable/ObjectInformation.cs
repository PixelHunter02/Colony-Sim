using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IStorable, IInteractable
{
    [SerializeField] private StoredItemSO _itemSO;
    public StoredItemSO Item
    {
        get => _itemSO;
    }
    public bool _isStored;
    public bool _isHeld;
    public Vector3 storageLocation;
    public Villager assignedVillager;
    
    public void OnInteraction()
    {
        if(!CanBeStored())
            return;

        FindAvailableVillager();
    }
    
    public void AssignStorage()
    {
        var location = StorageManager.storageLocations.ElementAt(0);
        storageLocation = location;
        StorageManager.UseStorageSpace(location);
    }
    
    private void FindAvailableVillager()
    {
        // foreach (var villager in VillagerManager.GetVillagers())
        // {
        //     if (villager.CurrentState == VillagerStates.Idle &&
        //         villager.TryGetComponent(out TaskHandler taskHandler) && !_isHeld && !villager.currentlyHolding && assignedVillager == null)
        //     {
        //         AssignStorage();
        //         assignedVillager = villager;
        //         StartCoroutine(taskHandler.PickUpResource(villager, this));
        //         break;
        //     }
        // }

        AssignStorage();
        foreach (var worker in VillagerManager.GetVillagers())
        {
            if (assignedVillager)
            {
                if (assignedVillager.TasksToQueue.Count > worker.TasksToQueue.Count ||
                    worker.CurrentState is VillagerStates.Idle)
                {
                    assignedVillager = worker;
                    Debug.Log(worker.TasksToQueue.Count + " : " + worker.VillagerName);
                }
            }
            else
            {
                assignedVillager = worker;
                Debug.Log(worker.TasksToQueue.Count + " : " + worker.VillagerName);

            }
        }
        if (assignedVillager && assignedVillager.TryGetComponent(out TaskHandler taskHandler))
        {
            assignedVillager.AddTaskToQueue(taskHandler.PickUpResource(assignedVillager, this));
            assignedVillager = null;
        }
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isHeld && !_isStored;
}
