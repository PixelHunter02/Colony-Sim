using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
    private GameManager _gameManager;
    // public List<IEnumerator> queuedTasks;
    public Queue<Coroutine> queuedTasks;
    // public LayerMask pickupLayerMask;
    public List<Roles> crafters;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        queuedTasks = new Queue<Coroutine>();
        crafters = new List<Roles>() { Roles.Crafter, Roles.Leader };
    }

    #region Assign

    public IEnumerator TaskToAssign(HarvestableObject task)
    {
        if (VillagerManager.TryGetVillagerByRole(task.harvestableObject.canInteract, out Villager villager))
        {
            villager.StopAllCoroutines();
            yield return StartCoroutine(RunTaskCR(villager, task));
            queuedTasks.Dequeue();
        }
        else
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(TaskToAssign(task));
        }
    }
    
    public IEnumerator TaskToAssign(ObjectInformation task)
    {
        if (VillagerManager.TryGetVillagerByRole(out Villager villager))
        {
            villager.StopAllCoroutines();
            yield return StartCoroutine(RunTaskCR(villager, task));
            queuedTasks.Dequeue();
        }
        else
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(TaskToAssign(task));
        }
    }
    
    public IEnumerator TaskToAssign(BuildStats task)
    {
        if (VillagerManager.TryGetVillagerByRole(crafters, out Villager villager))
        {
            villager.StopAllCoroutines();
            yield return StartCoroutine(RunTaskCR(villager, task));
            queuedTasks.Dequeue();
        }
        else
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(TaskToAssign(task));
        }
    }

    #endregion

    #region Harvest

    public IEnumerator RunTaskCR(Villager assignedVillager, HarvestableObject task)
    {
        
        yield return StartCoroutine(WalkToLocationCR(assignedVillager, task,4f));
        yield return StartCoroutine(VillagerDoesTaskCR(assignedVillager, task));
    }
    
    // Villager Does Task
    private IEnumerator VillagerDoesTaskCR(Villager assignedVillager, HarvestableObject task)
    {
        // Set Slider To be Visible
        GameObject sliderGo = assignedVillager.transform.Find("Canvas").Find("Slider").gameObject;
        sliderGo.SetActive(true);
        Slider slider = sliderGo.GetComponent<Slider>();
        var sprite = slider.transform.Find("Fill Area").Find("Inner").Find("CurrentTaskImage").GetComponent<Image>();
        sprite.sprite = task.harvestableObject.taskSprite;
        
        // Set The Villager To The Working State
        assignedVillager.CurrentState = VillagerStates.Working;

        // Get The Current Progress Of The Task
        float timer = 0;
        var duration = task.harvestableObject.timeToHarvest;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            var progress = timer / duration;
            slider.value = progress;
            yield return null;
        }
        
        

        // Show The Task Has Been Completed
        sprite.sprite = task.harvestableObject.taskCompleteSprite;
        StartCoroutine(task.CRSpawnHarvestDrops());
        Destroy(task.gameObject);
        
        //Set The Villager To Its  Idle State
        Villager.StopVillager(assignedVillager, false);
        assignedVillager.CurrentState = VillagerStates.Idle;
        
        // Disable The Canvas And Un-Assign Tasks
        yield return new WaitForSeconds(1.5f);
        // assignedVillager.interactingWith = null;
        sliderGo.SetActive(false);
    }

    #endregion

    #region Pickup Resources

    public IEnumerator RunTaskCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        yield return StartCoroutine(WalkToLocationCR(assignedVillager, resourceToPickUp,4f));
        yield return StartCoroutine(VillagerPicksUpItemCR(assignedVillager, resourceToPickUp));
        yield return StartCoroutine(VillagerWalksToStockpilePointCR(assignedVillager, resourceToPickUp));
    }
    
    //Worker Picks Up Item
    private IEnumerator VillagerPicksUpItemCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        // assignedVillager.currentlyHolding = resourceToPickUp.Item;
        resourceToPickUp._isHeld = true;
        assignedVillager.CurrentState = VillagerStates.Pickup;
        yield return new WaitForSeconds(1f);
        resourceToPickUp.gameObject.SetActive(false);
        yield return null;
    }
    
    //Worker walks to Stockpile Point
    private IEnumerator VillagerWalksToStockpilePointCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        var storageLocation = resourceToPickUp.storageLocation;

        yield return WalkToLocationCR(assignedVillager, storageLocation);
        
        assignedVillager.CurrentState = VillagerStates.Pickup;
        
        yield return new WaitForSeconds(0.5f);
        
        MoveObjectToStorage(assignedVillager, resourceToPickUp);
    }
    
    //Worker puts down Item
    private void MoveObjectToStorage(Villager assignedVillager, ObjectInformation objectInformation)
    {
        objectInformation.transform.position = objectInformation.storageLocation;
        objectInformation.gameObject.SetActive(true);
        objectInformation.transform.rotation = Quaternion.Euler(0,0,0);
        objectInformation._isHeld = false;
        // assignedVillager.currentlyHolding = null;
        objectInformation._isStored = true;
        _gameManager.storageManager.AddToStorage(new Item{itemSO = objectInformation.Item, amount = 1, storageLocation = objectInformation.storageLocation, go = objectInformation.gameObject});
        assignedVillager.CurrentState = VillagerStates.Idle;
    }

    #endregion

    #region Build

    public IEnumerator RunTaskCR(Villager assignedVillager, BuildStats buildStats)
    {
            List<Item> resourcesToRemove = new List<Item>();

            foreach (var required in buildStats.craftingRecipe.craftingRecipe)
            {
                if (StorageManager.TryFindItemsInInventory(required, required.amount, out List<Item> resources))
                {
                    foreach (var resource in resources)
                    {
                        Debug.Log(resource.itemSO.objectName);
                        resourcesToRemove.Add(resource);
                    }
                }
                else
                {
                    Debug.Log("You dont have required resources");
                    yield return new WaitForSeconds(5f);
                    Coroutine cr = StartCoroutine(_gameManager.taskHandler.TaskToAssign(buildStats));
                    _gameManager.taskHandler.queuedTasks.Enqueue(cr);
                    yield break;
                }
            }
            
            
            Debug.Log("Has Resources");

            foreach (var item in resourcesToRemove)
            {
                yield return StartCoroutine(PickUpItems(assignedVillager, item));
            }

            yield return WalkToLocationCR(assignedVillager, buildStats,2);
            yield return new WaitForSeconds(3);
            assignedVillager.CurrentState = VillagerStates.Idle;
            buildStats.building.SetActive(false);
            buildStats.built.SetActive(true);
    }
    
    private IEnumerator PickUpItems(Villager assignedVillager, Item location)
    {
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.storageLocation);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        // while (!Physics.Raycast(new Vector3(assignedVillager.transform.position.x,assignedVillager.transform.position.y+1.5f,assignedVillager.transform.position.z),Vector3.down*2,10,pickupLayerMask))
        // {
        //     yield return null;
        // }
        assignedVillager.CurrentState = VillagerStates.Pickup;
        Debug.Log("Hit");
        Villager.StopVillager(assignedVillager, true);
        yield return new WaitForSeconds(1f);
        
        StorageManager.EmptyStockpileSpace(location);
    }


    #endregion

    
    public IEnumerator WalkToLocationCR(Villager assignedVillager, Component location, float distance)
    {
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.transform.position);

        // Set the villagers state to walking if not already.
        if (assignedVillager.CurrentState != VillagerStates.Walking)
        {
            assignedVillager.CurrentState = VillagerStates.Walking;
        }
        
        while (Vector3.Distance(assignedVillager.transform.position, location.transform.position) > distance)
        {
            yield return null;
        }
        
        Villager.StopVillager(assignedVillager,true);

        yield return null;
    }
    
    public IEnumerator WalkToLocationCR(Villager assignedVillager, Vector3 location)
    {
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location);
        // Set the villagers state to walking if not already.
        if (assignedVillager.CurrentState != VillagerStates.Walking)
        {
            assignedVillager.CurrentState = VillagerStates.Walking;
        }
        
        while (Vector3.Distance(assignedVillager.transform.position, location) > 2f)
        {
            Debug.Log(Vector3.Distance(assignedVillager.transform.position, location));

            yield return null;
        }
        
        Villager.StopVillager(assignedVillager,true);

        yield return null;
    }
    
    
}

