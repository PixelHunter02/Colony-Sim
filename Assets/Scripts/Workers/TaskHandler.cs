using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG_Tasks;
using UnityEngine.AI;

public class TaskHandler : MonoBehaviour
{
    private GameManager _gameManager;
    public Queue<Coroutine> queuedTasks;
    public List<Roles> crafters;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        queuedTasks = new Queue<Coroutine>();
        crafters = new List<Roles>() { Roles.Crafter, Roles.Leader };
    }

    #region Assign

    public IEnumerator TaskToAssign(BuildStats task)
    {
        if (VillagerManager.TryGetVillagerByRole(out Villager villager,task.transform.position))
        {
            yield return StartCoroutine(RunTaskCR(villager, task));
            queuedTasks.Dequeue();
        }
        else
        {
            Debug.Log("Unable To Find Villager, Retrying in 3 Seconds.");
            yield return new WaitForSeconds(3f);
            StartCoroutine(TaskToAssign(task));
        }
    }

    #endregion

    #region Harvest

    public IEnumerator RunTaskCR(Villager assignedVillager, HarvestableObject task)
    {
        
        yield return StartCoroutine(Tasks.WalkToLocation(assignedVillager, task.standPoint.position, () => {task.harvestParticle.Play();}));
        yield return StartCoroutine(Tasks.WalkToLocation(assignedVillager, task.standPoint.position, () => { task.audioSource.Play(); }));
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
        yield return StartCoroutine(task.CRSpawnHarvestDrops());
        Destroy(task.gameObject);
        
        // Set The Villager To Its Idle State
        Villager.StopVillager(assignedVillager, false);
        assignedVillager.CurrentState = VillagerStates.Idle;
        
        // Disable The Canvas And Un-Assign Tasks
        yield return new WaitForSeconds(1.5f);
        sliderGo.SetActive(false);
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

            yield return Tasks.WalkToLocation(assignedVillager, buildStats.transform.position);
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
       
        assignedVillager.CurrentState = VillagerStates.Pickup;
        Debug.Log("Hit");
        Villager.StopVillager(assignedVillager, true);
        yield return new WaitForSeconds(1f);
        
        StorageManager.EmptyStockpileSpace(location);
    }
    #endregion
    
    public IEnumerator WalkToLocationCR(Villager assignedVillager, Vector3 location, Action onArrivedAtPosition = null)
    {
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location);
        // Set the villagers state to walking if not already.
        if (assignedVillager.CurrentState != VillagerStates.Walking)
        {
            assignedVillager.CurrentState = VillagerStates.Walking;
        }
        
        while (Vector3.Distance(assignedVillager.transform.position, location) > 3f)
        {
            Debug.Log(Vector3.Distance(assignedVillager.transform.position, location));

            yield return null;
        }
        
        Villager.StopVillager(assignedVillager,true);

        onArrivedAtPosition?.Invoke();
        yield return null;
    }
    
    public static bool CanReachPosition(Vector3 position, NavMeshAgent agent)
    {
        var path = new NavMeshPath();
        agent.CalculatePath(position, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            Debug.Log($"{agent.name} will be able to reach {position}.");
            return true;
        }
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            Debug.Log($"{agent.name} will only be able to move partway to {position}.");
            return false;
        }
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log($"There is no valid path for {agent.name} to reach {position}.");
            return false;
        }

        return false;
    }
}

