using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image taskImage;
    [SerializeField] private GameObject status;
    [SerializeField] private Worker _worker;
    private Animator _animator;

    public static void BeginWorking(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        harvestObjectManager.assignedWorker = worker;
        worker.interactingWith = harvestObjectManager;
        Worker.StopWorker(worker,false);
        Worker.ChangeWorkerState(worker, Worker.WorkerStates.Working);
    }
    
    public void BeginHarvest(HarvestObjectManager harvestObjectManager)
    {
        StartCoroutine(CRTaskTimer(harvestObjectManager));
        taskImage.sprite = harvestObjectManager.harvestableObject.taskSprite;
    }
    
    public void BeginWalkingToObject(Worker worker, PickupObject pickupObject)
    {
        Worker.ChangeWorkerState(worker,Worker.WorkerStates.Working);
        Worker.StopWorker(worker,false);
        Worker.SetWorkerDestination(worker, pickupObject.transform.position);
        StartCoroutine(CRWalkToPickup(worker, pickupObject));
    }
    
    public IEnumerator MoveToJob(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        BeginWorking(worker,harvestObjectManager);
        if (Vector3.Distance(transform.position, harvestObjectManager.transform.position) > 3f)
        {
            Worker.SetWorkerDestination(worker,harvestObjectManager.transform.position);
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(MoveToJob(worker, harvestObjectManager));
        }
        else
        {
            Worker.StopWorker(worker,true);
            BeginHarvest(harvestObjectManager);
            yield return null;
        }
    }
    
    private IEnumerator CRTaskTimer(HarvestObjectManager harvestObjectManager)
    {
        status.SetActive(true);
        
        var timer = 0f;
        var timeToCount = harvestObjectManager.harvestableObject.timeToHarvest;
        while (timer < timeToCount)
        {
            timer += Time.deltaTime;
            slider.value = timer / timeToCount;
            yield return null;
        }
        
        taskImage.sprite = harvestObjectManager.harvestableObject.taskCompleteSprite;
        StartCoroutine(harvestObjectManager.SpawnHarvestDrops());
        EndCurrentTask(_worker,harvestObjectManager);
        yield return new WaitForSeconds(3f);
        status.SetActive(false);
    }

    private IEnumerator CRWalkToPickup(Worker worker, PickupObject objectToPickUp)
    {
        var pickupPosition = objectToPickUp.transform.position;
        if (Vector3.Distance(worker.transform.position, pickupPosition) > 3f)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CRWalkToPickup(worker, objectToPickUp));
        }
        else
        {
            objectToPickUp.gameObject.SetActive(false);
            Worker.SetWorkerDestination(worker,objectToPickUp.storedAt);
            StartCoroutine(CRWalkToStockpile(worker, objectToPickUp));
        }
        yield return null;
    }

    private IEnumerator CRWalkToStockpile(Worker worker, PickupObject objectToStore)
    {
        if (Vector3.Distance(worker.transform.position, objectToStore.storedAt) > 3)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CRWalkToStockpile(worker, objectToStore));
        }
        else
        {
            objectToStore.transform.position = objectToStore.storedAt;
            objectToStore.gameObject.SetActive(true);
            Worker.ChangeWorkerState(worker, Worker.WorkerStates.Idle);
        }
    }
    private static void EndCurrentTask(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        harvestObjectManager.assignedWorker = null;
        worker.interactingWith = null;
        Worker.ChangeWorkerState( worker,Worker.WorkerStates.Idle);
    }
}
