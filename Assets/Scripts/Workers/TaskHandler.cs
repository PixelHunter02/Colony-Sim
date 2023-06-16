using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image taskImage;
    [SerializeField] private GameObject status;
    [SerializeField] private Worker _worker;
 
    
    public IEnumerator CRRunTask(HarvestObjectManager harvestObjectManager)
    {
        status.SetActive(true);
        var timer = 0f;
        var timeToCount = harvestObjectManager.harvestableObject.timeToHarvest;
        taskImage.sprite = harvestObjectManager.harvestableObject.taskSprite;
        while (timer < timeToCount)
        {
            timer += Time.deltaTime;
            slider.value = timer / timeToCount;
            yield return null;
        }
        
        taskImage.sprite = harvestObjectManager.harvestableObject.taskCompleteSprite;
        StartCoroutine(harvestObjectManager.CRSpawnHarvestDrops());
        Worker.StopWorker(_worker, false);
        Worker.SetWorkerState(_worker, WorkerStates.Idle);
        yield return new WaitForSeconds(3f);
        status.SetActive(false);
        Worker.SetInteractingWith(_worker, null);
        harvestObjectManager.assignedWorker = null;
        
    }

    public IEnumerator CRWalkToTask(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        if (Vector3.Distance(transform.position, harvestObjectManager.transform.position) > 3f)
        {
            if (Worker.GetWorkerState(worker) != WorkerStates.Walking)
            {
                // Set the Workers State To Walking
                Worker.SetWorkerState(worker,WorkerStates.Walking);
            }
            Worker.StopWorker(worker, false);
            Worker.SetWorkerDestination(worker,harvestObjectManager.transform.position);

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(CRWalkToTask(worker, harvestObjectManager));
        }
        else
        {
            Worker.StopWorker(worker,true);
            Worker.SetWorkerState(worker, WorkerStates.Working);
            StartCoroutine(CRRunTask(harvestObjectManager));
            yield return null;
        }
    }
    
    public IEnumerator CRWalkToPickup(Worker worker, PickupObject objectToPickUp)
    {
        var pickupPosition = objectToPickUp.transform.position;
        if (Vector3.Distance(worker.transform.position, pickupPosition) > 3f)
        {
            Worker.SetWorkerState(worker,WorkerStates.Walking);
            Worker.StopWorker(worker, false);
            Worker.SetWorkerDestination(worker, pickupPosition);

            // Repeat
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CRWalkToPickup(worker, objectToPickUp));
        }
        else
        {
            // Hide The Pickup Object
            objectToPickUp.gameObject.SetActive(false);
            
            Worker.SetWorkerDestination(worker, objectToPickUp.storedAt);


            StartCoroutine(CRWalkToStockpile(worker, objectToPickUp));
        }
        yield return null;
    }

    private IEnumerator CRWalkToStockpile(Worker worker, PickupObject objectToStore)
    {
        // Check the Distance From The Stockpile Position
        if (Vector3.Distance(worker.transform.position, objectToStore.storedAt) > 1)
        {
            // Set the Workers State To Walking
            Worker.SetWorkerState(worker,WorkerStates.Walking);

            // Repeat
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CRWalkToStockpile(worker, objectToStore));
        }
        else
        {

            // Move Object To Storage
            objectToStore.transform.position = objectToStore.storedAt;
            objectToStore.gameObject.SetActive(true);
            objectToStore.transform.rotation = Quaternion.Euler(0,0,0);

            // Set The Worker State To Idle
            Worker.SetWorkerState(worker, WorkerStates.Idle);
        }
    }
}
